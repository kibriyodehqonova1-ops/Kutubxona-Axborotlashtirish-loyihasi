using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Librarian")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Purchase
        public async Task<IActionResult> Index(string? holat, string? keywords, int page = 1)
        {
            const int pageSize = 20;

            var query = _context.Orders
                .Include(o => o.Foydalanuvchi)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Kitob)
                .AsQueryable();

            if (!string.IsNullOrEmpty(holat))
                query = query.Where(o => o.Holati == holat);

            if (!string.IsNullOrEmpty(keywords))
                query = query.Where(o =>
                    (o.Foydalanuvchi != null &&
                     (o.Foydalanuvchi.Ism!.Contains(keywords) ||
                      o.Foydalanuvchi.Familiya!.Contains(keywords) ||
                      o.Foydalanuvchi.Email!.Contains(keywords))) ||
                    o.OrderItems.Any(oi => oi.Kitob != null && oi.Kitob.Nomi.Contains(keywords)));

            int total = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.Sana)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.TotalCount = total;
            ViewBag.Holat = holat;
            ViewBag.Keywords = keywords;

            // Statistika
            ViewBag.KutilmoqdaCount = await _context.Orders.CountAsync(o => o.Holati == "Kutilmoqda");
            ViewBag.TolananCount = await _context.Orders.CountAsync(o => o.Holati == "To'langan");
            ViewBag.BekorCount = await _context.Orders.CountAsync(o => o.Holati == "Bekor qilingan");
            ViewBag.JamiDaromad = await _context.Orders
                .Where(o => o.Holati == "To'langan")
                .SumAsync(o => (decimal?)o.JamiSumma) ?? 0;

            return View(orders);
        }

        // GET: Admin/Purchase/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Foydalanuvchi)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Kitob)
                        .ThenInclude(k => k!.KitobMualliflari)
                            .ThenInclude(km => km.Muallif)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Admin/Purchase/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            if (order.Holati != "Kutilmoqda")
            {
                TempData["Error"] = "Faqat 'Kutilmoqda' holatidagi buyurtmani tasdiqlash mumkin.";
                return RedirectToAction("Details", new { id });
            }

            order.Holati = "To'langan";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Buyurtma #{id} tasdiqlandi va to'langan deb belgilandi.";
            return RedirectToAction("Details", new { id });
        }

        // POST: Admin/Purchase/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? sabab)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            if (order.Holati == "To'langan")
            {
                TempData["Error"] = "To'langan buyurtmani bekor qilib bo'lmaydi.";
                return RedirectToAction("Details", new { id });
            }

            order.Holati = "Bekor qilingan";
            if (!string.IsNullOrEmpty(sabab))
                order.Izoh = (order.Izoh != null ? order.Izoh + " | " : "") + "Bekor sababi: " + sabab;

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

            TempData["Success"] = $"Buyurtma #{id} bekor qilindi. Nusxalar qaytarildi.";
            return RedirectToAction("Details", new { id });
        }
    }
}
