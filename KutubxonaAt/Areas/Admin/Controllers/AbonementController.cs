using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Librarian")]
    public class AbonementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AbonementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== Abonement Turlari ====================

        public async Task<IActionResult> Turlar()
        {
            var turlar = await _context.AbonementTurlari
                .OrderBy(t => t.Narxi)
                .ToListAsync();
            return View(turlar);
        }

        [HttpGet]
        public IActionResult TurYaratish() => View(new AbonementTuri());

        [HttpPost]
        public async Task<IActionResult> TurYaratish(AbonementTuri model)
        {
            if (!ModelState.IsValid) return View(model);
            model.YaratilganSana = DateTime.Now;
            _context.AbonementTurlari.Add(model);
            await _context.SaveChangesAsync();
            TempData["Xabar"] = "Abonement turi yaratildi.";
            return RedirectToAction("Turlar");
        }

        [HttpGet]
        public async Task<IActionResult> TurTahrirla(int id)
        {
            var tur = await _context.AbonementTurlari.FindAsync(id);
            if (tur == null) return NotFound();
            return View(tur);
        }

        [HttpPost]
        public async Task<IActionResult> TurTahrirla(AbonementTuri model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.AbonementTurlari.Update(model);
            await _context.SaveChangesAsync();
            TempData["Xabar"] = "Abonement turi yangilandi.";
            return RedirectToAction("Turlar");
        }

        [HttpPost]
        public async Task<IActionResult> TurHolat(int id)
        {
            var tur = await _context.AbonementTurlari.FindAsync(id);
            if (tur == null) return NotFound();
            tur.Faol = !tur.Faol;
            await _context.SaveChangesAsync();
            return RedirectToAction("Turlar");
        }

        // ==================== Foydalanuvchi Abonementlari ====================

        public async Task<IActionResult> Index(string? holat, string? qidiruv)
        {
            var query = _context.Abonementlar
                .Include(a => a.AbonementTuri)
                .Include(a => a.Foydalanuvchi)
                .AsQueryable();

            if (!string.IsNullOrEmpty(holat))
                query = query.Where(a => a.Holati == holat);

            if (!string.IsNullOrEmpty(qidiruv))
                query = query.Where(a =>
                    a.Foydalanuvchi!.Ism.Contains(qidiruv) ||
                    a.Foydalanuvchi.Familiya.Contains(qidiruv) ||
                    a.Foydalanuvchi.Email!.Contains(qidiruv));

            ViewBag.JamiSoni = await _context.Abonementlar.CountAsync();
            ViewBag.FaolSoni = await _context.Abonementlar.CountAsync(a => a.Holati == "Faol" && a.TugashSana > DateTime.Now);
            ViewBag.TugaganSoni = await _context.Abonementlar.CountAsync(a => a.Holati == "Tugagan" || a.TugashSana <= DateTime.Now);

            var abonementlar = await query
                .OrderByDescending(a => a.BoshlanishSana)
                .Take(200)
                .ToListAsync();

            return View(abonementlar);
        }

        [HttpPost]
        public async Task<IActionResult> BekorQilish(int id)
        {
            var ab = await _context.Abonementlar.FindAsync(id);
            if (ab == null) return NotFound();
            ab.Holati = "Bekor qilingan";
            await _context.SaveChangesAsync();
            TempData["Xabar"] = "Abonement bekor qilindi.";
            return RedirectToAction("Index");
        }
    }
}
