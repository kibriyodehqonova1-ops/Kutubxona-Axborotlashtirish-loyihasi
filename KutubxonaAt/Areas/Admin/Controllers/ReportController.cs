using KutubxonaAt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bugun = DateTime.Today;
            var oyBoshi = new DateTime(bugun.Year, bugun.Month, 1);
            var yilBoshi = new DateTime(bugun.Year, 1, 1);

            // ===== Kitoblar =====
            ViewBag.KopBronKitoblar = await _context.Bronlar
                .Where(b => b.BronSana >= oyBoshi)
                .GroupBy(b => b.KitobId)
                .Select(g => new { KitobId = g.Key, Son = g.Count() })
                .OrderByDescending(g => g.Son)
                .Take(10)
                .Join(_context.Kitoblar, g => g.KitobId, k => k.Id, (g, k) => new { k.Nomi, g.Son })
                .ToListAsync();

            ViewBag.KopIjaragaOligan = await _context.Beruvlar
                .Where(b => b.BerilganSana >= oyBoshi)
                .GroupBy(b => b.KitobNusxa!.KitobId)
                .Select(g => new { KitobId = g.Key, Son = g.Count() })
                .OrderByDescending(g => g.Son)
                .Take(10)
                .Join(_context.Kitoblar, g => g.KitobId, k => k.Id, (g, k) => new { k.Nomi, g.Son })
                .ToListAsync();

            ViewBag.KopKechiktirgan = await _context.Beruvlar
                .Where(b => b.Holati == "MuddatiO'tgan" || b.QaytarilganSana > b.QaytarishMuddati)
                .GroupBy(b => b.FoydalanuvchiId)
                .Select(g => new { UserId = g.Key, Son = g.Count() })
                .OrderByDescending(g => g.Son)
                .Take(10)
                .Join(_context.Users, g => g.UserId, u => u.Id,
                    (g, u) => new { Ism = u.Ism + " " + u.Familiya, g.Son })
                .ToListAsync();

            // ===== Statistika =====
            ViewBag.JamiBeruvlar = await _context.Beruvlar.CountAsync();
            ViewBag.OylikBeruvlar = await _context.Beruvlar.CountAsync(b => b.BerilganSana >= oyBoshi);
            ViewBag.FaolBeruvlar = await _context.Beruvlar
                .CountAsync(b => b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan");
            ViewBag.MuddatiOtgan = await _context.Beruvlar
                .CountAsync(b => b.Holati == "MuddatiO'tgan");

            ViewBag.JamiJarimalar = await _context.Jarimalar
                .Where(j => j.Holati == "To'langan")
                .SumAsync(j => (decimal?)j.Miqdor) ?? 0;
            ViewBag.KutilayotganJarimalar = await _context.Jarimalar
                .Where(j => j.Holati == "Kutilmoqda")
                .SumAsync(j => (decimal?)j.Miqdor) ?? 0;

            ViewBag.FaolAbonementlar = await _context.Abonementlar
                .CountAsync(a => a.Holati == "Faol" && a.TugashSana > DateTime.Now);
            ViewBag.AbonementDaromad = await _context.Abonementlar
                .Where(a => a.BoshlanishSana >= yilBoshi)
                .SumAsync(a => (decimal?)a.TolanganNarx) ?? 0;

            ViewBag.JamiKitoblar = await _context.Kitoblar.CountAsync();
            ViewBag.JamiNusxalar = await _context.KitobNusxalari.CountAsync();
            ViewBag.MavjudNusxalar = await _context.KitobNusxalari.CountAsync(n => n.Holati == "Mavjud");

            // Oylik grafik ma'lumoti (so'nggi 6 oy)
            var oylikBeruvlar = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var oy = DateTime.Today.AddMonths(-i);
                var oyBoshiLocal = new DateTime(oy.Year, oy.Month, 1);
                var oyOxiri = oyBoshiLocal.AddMonths(1);
                var son = await _context.Beruvlar
                    .CountAsync(b => b.BerilganSana >= oyBoshiLocal && b.BerilganSana < oyOxiri);
                oylikBeruvlar.Add(new { Oy = oy.ToString("MMM yyyy"), Son = son });
            }
            ViewBag.OylikGrafik = oylikBeruvlar;

            return View();
        }

        public async Task<IActionResult> Foydalanuvchilar()
        {
            var foydalanuvchilar = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Ism,
                    u.Familiya,
                    u.Email,
                    u.Turi,
                    u.Faol,
                    BeruvlarSoni = _context.Beruvlar.Count(b => b.FoydalanuvchiId == u.Id),
                    JarimalarSoni = _context.Jarimalar.Count(j => j.FoydalanuvchiId == u.Id && j.Holati == "Kutilmoqda"),
                    FaolAbonement = _context.Abonementlar.Any(a => a.FoydalanuvchiId == u.Id && a.Holati == "Faol" && a.TugashSana > DateTime.Now)
                })
                .OrderByDescending(u => u.BeruvlarSoni)
                .ToListAsync();

            return View(foydalanuvchilar);
        }

        public async Task<IActionResult> Moliyaviy(int yil = 0)
        {
            if (yil == 0) yil = DateTime.Today.Year;
            var yilBoshi = new DateTime(yil, 1, 1);
            var yilOxiri = yilBoshi.AddYears(1);

            var oylikHisobot = new List<object>();
            for (int oy = 1; oy <= 12; oy++)
            {
                var oyBoshi = new DateTime(yil, oy, 1);
                var oyOxiri = oyBoshi.AddMonths(1);

                var jarima = await _context.Jarimalar
                    .Where(j => j.TolanganSana >= oyBoshi && j.TolanganSana < oyOxiri && j.Holati == "To'langan")
                    .SumAsync(j => (decimal?)j.Miqdor) ?? 0;

                var abonement = await _context.Abonementlar
                    .Where(a => a.BoshlanishSana >= oyBoshi && a.BoshlanishSana < oyOxiri)
                    .SumAsync(a => (decimal?)a.TolanganNarx) ?? 0;

                var sotuv = await _context.Sotuvlar
                    .Where(s => s.SotibOlinganSana >= oyBoshi && s.SotibOlinganSana < oyOxiri)
                    .SumAsync(s => (decimal?)s.TolanganNarx) ?? 0;

                oylikHisobot.Add(new
                {
                    Oy = oyBoshi.ToString("MMMM"),
                    Jarima = jarima,
                    Abonement = abonement,
                    Sotuv = sotuv,
                    Jami = jarima + abonement + sotuv
                });
            }

            ViewBag.OylikHisobot = oylikHisobot;
            ViewBag.Yil = yil;
            ViewBag.JamiDaromad = (decimal)oylikHisobot.Sum(o => (decimal)o.GetType().GetProperty("Jami")!.GetValue(o)!);

            return View();
        }
    }
}
