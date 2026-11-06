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
    public class AbonementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBildirishnomaService _bildirishnoma;

        public AbonementController(ApplicationDbContext context, IBildirishnomaService bildirishnoma)
        {
            _context = context;
            _bildirishnoma = bildirishnoma;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var faolAbonement = await _context.Abonementlar
                .Where(a => a.FoydalanuvchiId == userId && a.Holati == "Faol")
                .Include(a => a.AbonementTuri)
                .OrderByDescending(a => a.BoshlanishSana)
                .FirstOrDefaultAsync();

            var tarix = await _context.Abonementlar
                .Where(a => a.FoydalanuvchiId == userId && a.Holati != "Faol")
                .Include(a => a.AbonementTuri)
                .OrderByDescending(a => a.BoshlanishSana)
                .Take(5)
                .ToListAsync();

            var turlar = await _context.AbonementTurlari
                .Where(t => t.Faol)
                .OrderBy(t => t.Narxi)
                .ToListAsync();

            ViewBag.FaolAbonement = faolAbonement;
            ViewBag.Tarix = tarix;
            ViewBag.Turlar = turlar;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sotib(int turId, string tolovUsuli = "Naqd")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Faol abonement bormi?
            var faolBormi = await _context.Abonementlar
                .AnyAsync(a => a.FoydalanuvchiId == userId && a.Holati == "Faol" && a.TugashSana > DateTime.Now);
            if (faolBormi)
            {
                TempData["Xabar"] = "Sizda faol abonement mavjud. Muddati tugagandan so'ng yangilash mumkin.";
                TempData["XabarTuri"] = "warning";
                return RedirectToAction("Index");
            }

            var tur = await _context.AbonementTurlari.FindAsync(turId);
            if (tur == null || !tur.Faol)
            {
                TempData["Xabar"] = "Tanlangan abonement turi topilmadi.";
                TempData["XabarTuri"] = "danger";
                return RedirectToAction("Index");
            }

            var abonement = new Abonement
            {
                FoydalanuvchiId = userId!,
                AbonementTuriId = turId,
                BoshlanishSana = DateTime.Now,
                TugashSana = DateTime.Now.AddDays(tur.DavomiylikKun),
                TolanganNarx = tur.Narxi,
                Holati = "Faol",
                TolovUsuli = tolovUsuli,
                YaratilganSana = DateTime.Now
            };
            _context.Abonementlar.Add(abonement);

            // ApplicationUser muddatini yangilash
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
                user.AmalMuddati = abonement.TugashSana;

            await _context.SaveChangesAsync();

            await _bildirishnoma.YuborAsync(
                userId!,
                "🎉 Abonement faollashtirildi!",
                $"'{tur.Nomi}' abonementingiz muvaffaqiyatli faollashtirildi. " +
                $"Muddati: {abonement.BoshlanishSana:dd.MM.yyyy} — {abonement.TugashSana:dd.MM.yyyy}. " +
                $"Oyiga {tur.BepulKitoblarSoni} ta kitob bepul olishingiz mumkin.",
                "Abonement"
            );

            TempData["Xabar"] = $"'{tur.Nomi}' abonement muvaffaqiyatli faollashtirildi!";
            TempData["XabarTuri"] = "success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Bekor(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var abonement = await _context.Abonementlar
                .Include(a => a.AbonementTuri)
                .FirstOrDefaultAsync(a => a.Id == id && a.FoydalanuvchiId == userId);

            if (abonement == null) return NotFound();

            abonement.Holati = "Bekor qilingan";
            await _context.SaveChangesAsync();

            TempData["Xabar"] = "Abonement bekor qilindi.";
            TempData["XabarTuri"] = "info";
            return RedirectToAction("Index");
        }
    }
}
