using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Kitob)
                .Where(o => o.FoydalanuvchiId == userId)
                .OrderByDescending(o => o.Sana)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Kitob)
                .FirstOrDefaultAsync(o => o.Id == id && o.FoydalanuvchiId == userId);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: User/Purchase/DigitalLibrary — xarid qilingan digital materiallar
        public async Task<IActionResult> DigitalLibrary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // To'langan buyurtmalardagi barcha digital materiallarni olamiz
            var items = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Kitob)
                    .ThenInclude(k => k!.KitobMualliflari)
                        .ThenInclude(km => km.Muallif)
                .Where(oi => oi.Order!.FoydalanuvchiId == userId
                          && oi.Order.Holati == "To'langan"
                          && oi.Kitob != null
                          && (oi.Kitob.MediaTuri == MediaType.ElektronKitob
                           || oi.Kitob.MediaTuri == MediaType.AudioKitob
                           || oi.Kitob.MediaTuri == MediaType.Video))
                .OrderByDescending(oi => oi.Order!.Sana)
                .ToListAsync();

            // Takrorlanishlarni olib tashlaymiz (bir kitobni ikki marta xarid qilgan bo'lsa)
            var uniqueItems = items
                .GroupBy(oi => oi.KitobId)
                .Select(g => g.First())
                .ToList();

            ViewBag.EKitoblar = uniqueItems
                .Where(oi => oi.Kitob!.MediaTuri == MediaType.ElektronKitob).ToList();
            ViewBag.Audiolar  = uniqueItems
                .Where(oi => oi.Kitob!.MediaTuri == MediaType.AudioKitob).ToList();
            ViewBag.Videolar  = uniqueItems
                .Where(oi => oi.Kitob!.MediaTuri == MediaType.Video).ToList();

            return View(uniqueItems);
        }

        // GET: User/Purchase/Download/{kitobId} — yuklab olish (tekshirib keyin yo'naltirish)
        public async Task<IActionResult> Download(int kitobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Foydalanuvchi bu materialni to'lab xarid qilganmi?
            var purchased = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Kitob)
                .AnyAsync(oi => oi.KitobId == kitobId
                             && oi.Order!.FoydalanuvchiId == userId
                             && oi.Order.Holati == "To'langan");

            if (!purchased)
            {
                TempData["Error"] = "Bu materialni yuklab olish uchun avval sotib olishingiz kerak.";
                return RedirectToAction("Details", "Book", new { area = "User", id = kitobId });
            }

            var kitob = await _context.Kitoblar.FindAsync(kitobId);
            if (kitob == null || string.IsNullOrEmpty(kitob.ElektronManzil))
            {
                TempData["Error"] = "Material fayli topilmadi.";
                return RedirectToAction("DigitalLibrary");
            }

            // Yuklashlar sonini oshiramiz (statistika)
            kitob.KorishlarSoni++;
            await _context.SaveChangesAsync();

            // Tashqi URL ga yo'naltirish
            return Redirect(kitob.ElektronManzil);
        }

        // POST: User/Purchase/ConfirmPayment — simulyatsiya orqali to'lovni tasdiqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id, string tolovUsuli)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.FoydalanuvchiId == userId);

            if (order == null) return NotFound();

            if (order.Holati != "Kutilmoqda")
            {
                TempData["Error"] = "Bu buyurtma allaqachon qayta ishlangan.";
                return RedirectToAction("Details", new { id });
            }

            order.Holati = "To'langan";
            if (!string.IsNullOrEmpty(tolovUsuli))
                order.TolovUsuli = tolovUsuli;

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ To'lov muvaffaqiyatli qabul qilindi! Buyurtma #{id} tasdiqlandi.";
            return RedirectToAction("Details", new { id });
        }

        // POST: User/Purchase/Cancel — foydalanuvchi tomonidan bekor qilish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.FoydalanuvchiId == userId);

            if (order == null) return NotFound();

            if (order.Holati != "Kutilmoqda")
            {
                TempData["Error"] = "Faqat 'Kutilmoqda' holatidagi buyurtmani bekor qilish mumkin.";
                return RedirectToAction("Details", new { id });
            }

            order.Holati = "Bekor qilingan";
            order.Izoh = (order.Izoh != null ? order.Izoh + " | " : "") + "Foydalanuvchi tomonidan bekor qilindi.";

            // Nusxalarni qaytarish
            foreach (var item in order.OrderItems)
            {
                var nusxa = await _context.KitobNusxalari.FindAsync(item.KitobNusxaId);
                if (nusxa != null && nusxa.Holati == "Sotilgan")
                {
                    nusxa.Holati = "Mavjud";
                    nusxa.SotuvgaQoyilgan = true;
                    _context.Update(nusxa);

                    var kitob = await _context.Kitoblar.FindAsync(item.KitobId);
                    if (kitob != null) { kitob.MavjudNusxalar++; _context.Update(kitob); }
                }
            }

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Buyurtma #{id} bekor qilindi.";
            return RedirectToAction("Index");
        }
    }
}