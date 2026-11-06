using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class KutishRoyxatiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KutishRoyxatiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var royxat = await _context.KutishRoyxati
                .Where(k => k.FoydalanuvchiId == userId && k.Holati == "Kutmoqda")
                .Include(k => k.Kitob)
                    .ThenInclude(ki => ki!.KitobMualliflari)
                        .ThenInclude(km => km.Muallif)
                .OrderBy(k => k.QoshilganSana)
                .ToListAsync();

            // Har bir kitob uchun eng erta qaytish sanasini hisoblash
            var kitobIdlar = royxat.Select(r => r.KitobId).Distinct().ToList();
            var qaytishSanalar = await _context.Beruvlar
                .Where(b => kitobIdlar.Contains(b.KitobNusxa!.KitobId)
                    && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan"))
                .GroupBy(b => b.KitobNusxa!.KitobId)
                .Select(g => new { KitobId = g.Key, EngErtaSana = g.Min(b => b.QaytarishMuddati) })
                .ToDictionaryAsync(g => g.KitobId, g => g.EngErtaSana);

            ViewBag.QaytishSanalar = qaytishSanalar;
            return View(royxat);
        }

        [HttpPost]
        public async Task<IActionResult> Qoshish(int kitobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Allaqachon bor?
            var bormi = await _context.KutishRoyxati
                .AnyAsync(k => k.FoydalanuvchiId == userId
                    && k.KitobId == kitobId
                    && k.Holati == "Kutmoqda");
            if (bormi)
            {
                TempData["Xabar"] = "Siz allaqachon bu kitobning kutish ro'yxatidasiz.";
                TempData["XabarTuri"] = "warning";
                return RedirectToAction("Details", "Book", new { area = "User", id = kitobId });
            }

            // Aslida nusxa bormi tekshirish
            var mavjudNusxa = await _context.KitobNusxalari
                .AnyAsync(n => n.KitobId == kitobId && n.Holati == "Mavjud" && !n.OquvZalidanChiqmaydi);
            if (mavjudNusxa)
            {
                TempData["Xabar"] = "Bu kitob hozir mavjud. Bron qilishingiz mumkin.";
                TempData["XabarTuri"] = "info";
                return RedirectToAction("Create", "Reservation", new { area = "User", kitobId });
            }

            var tartib = await _context.KutishRoyxati
                .CountAsync(k => k.KitobId == kitobId && k.Holati == "Kutmoqda") + 1;

            _context.KutishRoyxati.Add(new KutishRoyxati
            {
                FoydalanuvchiId = userId!,
                KitobId = kitobId,
                QoshilganSana = DateTime.Now,
                Holati = "Kutmoqda",
                Tartib = tartib
            });
            await _context.SaveChangesAsync();

            // Eng erta qaytish sanasini topish
            var engErtaSana = await _context.Beruvlar
                .Where(b => b.KitobNusxa!.KitobId == kitobId
                    && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan"))
                .MinAsync(b => (DateTime?)b.QaytarishMuddati);

            var xabar = $"Kutish ro'yxatiga qo'shildingiz. Navbatingiz: #{tartib}.";
            if (engErtaSana.HasValue)
                xabar += $" Taxminiy mavjud bo'lish sanasi: {engErtaSana.Value:dd.MM.yyyy}.";

            TempData["Xabar"] = xabar;
            TempData["XabarTuri"] = "success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Bekor(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var yozuv = await _context.KutishRoyxati
                .FirstOrDefaultAsync(k => k.Id == id && k.FoydalanuvchiId == userId);

            if (yozuv == null)
                return NotFound();

            yozuv.Holati = "Bekor qilingan";
            await _context.SaveChangesAsync();

            TempData["Xabar"] = "Kutish ro'yxatidan chiqarildi.";
            TempData["XabarTuri"] = "info";
            return RedirectToAction("Index");
        }
    }
}
