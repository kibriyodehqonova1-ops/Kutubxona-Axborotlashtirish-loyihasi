using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AutoBerishService _autoBerishService;
        private readonly IBildirishnomaService _bildirishnoma;

        public ReservationController(ApplicationDbContext context,
            AutoBerishService autoBerishService,
            IBildirishnomaService bildirishnoma)
        {
            _context = context;
            _autoBerishService = autoBerishService;
            _bildirishnoma = bildirishnoma;
        }

        // GET: /User/Reservation/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bronlar = await _context.Bronlar
                .Include(b => b.Kitob)
                .Include(b => b.KitobNusxa)
                .Where(b => b.FoydalanuvchiId == userId)
                .OrderByDescending(b => b.BronSana)
                .ToListAsync();

            return View(bronlar);
        }

        // GET: Kitob mavjud emasligida qaytish sanasi va kutish ro'yxati
        [HttpGet]
        public async Task<IActionResult> KitobMavjudligi(int kitobId)
        {
            var kitob = await _context.Kitoblar
                .Include(k => k.KitobNusxalari)
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .FirstOrDefaultAsync(k => k.Id == kitobId);

            if (kitob == null) return NotFound();

            var mavjudNusxa = kitob.KitobNusxalari.Any(n => n.Holati == "Mavjud" && !n.OquvZalidanChiqmaydi);

            // Eng erta qaytish sanasi
            DateTime? engErtaQaytish = null;
            if (!mavjudNusxa)
            {
                engErtaQaytish = await _context.Beruvlar
                    .Where(b => b.KitobNusxa!.KitobId == kitobId
                        && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan"))
                    .MinAsync(b => (DateTime?)b.QaytarishMuddati);
            }

            // Kutish ro'yxatidagi o'rin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kutishPozitsiya = await _context.KutishRoyxati
                .Where(k => k.KitobId == kitobId && k.Holati == "Kutmoqda")
                .CountAsync();

            var foydalanuvchiKutishda = userId != null && await _context.KutishRoyxati
                .AnyAsync(k => k.KitobId == kitobId && k.FoydalanuvchiId == userId && k.Holati == "Kutmoqda");

            ViewBag.Kitob = kitob;
            ViewBag.MavjudNusxa = mavjudNusxa;
            ViewBag.EngErtaQaytish = engErtaQaytish;
            ViewBag.KutishSoni = kutishPozitsiya;
            ViewBag.FoydalanuvchiKutishda = foydalanuvchiKutishda;

            return View();
        }

        // POST: Avtomatik bron va beruv
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int kitobId, bool yetkazibBerish = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account", new { area = "" });

            var kitob = await _context.Kitoblar
                .Include(k => k.KitobNusxalari)
                .FirstOrDefaultAsync(k => k.Id == kitobId);
            if (kitob == null)
            {
                TempData["Xabar"] = "Kitob topilmadi.";
                TempData["XabarTuri"] = "danger";
                return RedirectToAction("Index", "Book", new { area = "User" });
            }

            // Digital materiallar uchun bron qilish mumkin emas — faqat sotib olish
            bool isDigital = kitob.MediaTuri == Kitob.MediaType.ElektronKitob
                          || kitob.MediaTuri == Kitob.MediaType.AudioKitob
                          || kitob.MediaTuri == Kitob.MediaType.Video;
            if (isDigital)
            {
                TempData["Xabar"] = "Elektron, audio va video materiallarni bron qilib bo'lmaydi — ularni faqat sotib olish mumkin.";
                TempData["XabarTuri"] = "warning";
                return RedirectToAction("Details", "Book", new { area = "User", id = kitobId });
            }

            // Allaqachon faol bron bormi?
            var existingBron = await _context.Bronlar
                .FirstOrDefaultAsync(b => b.FoydalanuvchiId == userId
                    && b.KitobId == kitobId && b.Holati == "Faol");
            if (existingBron != null)
            {
                TempData["Xabar"] = "Siz bu kitobni allaqachon bron qilgansiz!";
                TempData["XabarTuri"] = "warning";
                return RedirectToAction("Index");
            }

            // Jarima tekshirish
            var jarimaBor = await _context.Jarimalar
                .AnyAsync(j => j.FoydalanuvchiId == userId && j.Holati == "Kutilmoqda");
            if (jarimaBor)
            {
                TempData["Xabar"] = "To'lanmagan jarimangiz mavjud. Avval jarimani to'lang.";
                TempData["XabarTuri"] = "danger";
                return RedirectToAction("Index", "Fine", new { area = "User" });
            }

            // Mavjud nusxani topish
            var mavjudNusxa = await _context.KitobNusxalari
                .FirstOrDefaultAsync(n => n.KitobId == kitobId
                    && n.Holati == "Mavjud"
                    && !n.OquvZalidanChiqmaydi);

            // Bron yaratish
            var bron = new Bron
            {
                FoydalanuvchiId = userId,
                KitobId = kitobId,
                KitobNusxaId = mavjudNusxa?.Id,
                BronSana = DateTime.Now,
                AmalMuddati = DateTime.Now.AddDays(3),
                Holati = "Faol",
                Izoh = "Sayt orqali bron qilingan"
            };
            _context.Bronlar.Add(bron);
            await _context.SaveChangesAsync();

            // Avtomatik beruv urinish
            var natija = await _autoBerishService.AvtomatikBerishAsync(bron.Id);

            if (natija.Muvaffaqiyatli)
            {
                // Yetkazib berish so'ralganda
                if (yetkazibBerish && natija.YaratilganBeruv != null)
                {
                    TempData["Xabar"] = natija.Xabar + " Yetkazib berish uchun quyida buyurtma bering.";
                    TempData["XabarTuri"] = "success";
                    return RedirectToAction("Buyurtma", "YetkazibBerish",
                        new { area = "User", beruvId = natija.YaratilganBeruv.Id });
                }

                TempData["Xabar"] = "✅ " + natija.Xabar;
                TempData["XabarTuri"] = "success";
            }
            else
            {
                TempData["Xabar"] = natija.Xabar;
                TempData["XabarTuri"] = natija.Xabar.Contains("kutish ro'yxati") ? "info" : "warning";
            }

            return RedirectToAction("Index");
        }

        // POST: /User/Reservation/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bron = await _context.Bronlar.FindAsync(id);

            if (bron == null || bron.FoydalanuvchiId != userId)
            {
                TempData["Xabar"] = "Bron topilmadi.";
                TempData["XabarTuri"] = "danger";
                return RedirectToAction("Index");
            }

            bron.Holati = "Bekor qilingan";
            await _context.SaveChangesAsync();

            TempData["Xabar"] = "Bron bekor qilindi.";
            TempData["XabarTuri"] = "info";
            return RedirectToAction("Index");
        }
    }
}
