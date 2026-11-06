using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Librarian")]
    public class YetkazibBerishController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBildirishnomaService _bildirishnoma;

        public YetkazibBerishController(ApplicationDbContext context, IBildirishnomaService bildirishnoma)
        {
            _context = context;
            _bildirishnoma = bildirishnoma;
        }

        public async Task<IActionResult> Index(string? holat)
        {
            var query = _context.YetkazibBerishlar
                .Include(y => y.Foydalanuvchi)
                .Include(y => y.Beruv)
                    .ThenInclude(b => b!.KitobNusxa)
                        .ThenInclude(kn => kn!.Kitob)
                .Include(y => y.Bron)
                    .ThenInclude(b => b!.Kitob)
                .AsQueryable();

            if (!string.IsNullOrEmpty(holat))
                query = query.Where(y => y.Holati == holat);

            ViewBag.Tayyorlanmoqda = await _context.YetkazibBerishlar.CountAsync(y => y.Holati == "Tayyorlanmoqda");
            ViewBag.Yolda = await _context.YetkazibBerishlar.CountAsync(y => y.Holati == "Yo'lda");
            ViewBag.Yetkazildi = await _context.YetkazibBerishlar.CountAsync(y => y.Holati == "Yetkazildi");

            var yetkazishlar = await query
                .OrderByDescending(y => y.BuyurtmaSana)
                .ToListAsync();

            return View(yetkazishlar);
        }

        [HttpPost]
        public async Task<IActionResult> HolatOzgartir(int id, string yangiHolat, string? haydovchi)
        {
            var yetkazish = await _context.YetkazibBerishlar
                .Include(y => y.Foydalanuvchi)
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yetkazish == null) return NotFound();

            yetkazish.Holati = yangiHolat;
            if (!string.IsNullOrEmpty(haydovchi))
                yetkazish.HaydovchiMaʼlumoti = haydovchi;

            if (yangiHolat == "Yetkazildi")
            {
                yetkazish.YetkazilganSana = DateTime.Now;
                yetkazish.TolovQilinganmi = true;
            }

            await _context.SaveChangesAsync();

            // Foydalanuvchiga bildiruv
            string bildiruvMatn = yangiHolat switch
            {
                "Yo'lda" => $"Kitobingiz yo'lda! Kuzatuv: #{yetkazish.KuzatuvRaqami}. Haydovchi: {haydovchi}.",
                "Yetkazildi" => $"Kitobingiz yetkazildi! Xaridingiz uchun rahmat.",
                _ => $"Yetkazib berish holati yangilandi: {yangiHolat}."
            };

            await _bildirishnoma.YuborAsync(
                yetkazish.FoydalanuvchiId,
                $"🚚 Yetkazib berish: {yangiHolat}",
                bildiruvMatn,
                "Yetkazish"
            );

            TempData["Xabar"] = $"Holat '{yangiHolat}' ga yangilandi.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detallar(int id)
        {
            var yetkazish = await _context.YetkazibBerishlar
                .Include(y => y.Foydalanuvchi)
                .Include(y => y.Beruv)
                    .ThenInclude(b => b!.KitobNusxa)
                        .ThenInclude(kn => kn!.Kitob)
                .Include(y => y.Bron)
                    .ThenInclude(b => b!.Kitob)
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yetkazish == null) return NotFound();
            return View(yetkazish);
        }
    }
}
