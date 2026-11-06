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
    public class YetkazibBerishController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBildirishnomaService _bildirishnoma;

        public YetkazibBerishController(ApplicationDbContext context, IBildirishnomaService bildirishnoma)
        {
            _context = context;
            _bildirishnoma = bildirishnoma;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var yetkazishlar = await _context.YetkazibBerishlar
                .Where(y => y.FoydalanuvchiId == userId)
                .Include(y => y.Beruv)
                    .ThenInclude(b => b!.KitobNusxa)
                        .ThenInclude(kn => kn!.Kitob)
                .Include(y => y.Bron)
                    .ThenInclude(b => b!.Kitob)
                .OrderByDescending(y => y.BuyurtmaSana)
                .ToListAsync();

            return View(yetkazishlar);
        }

        [HttpGet]
        public async Task<IActionResult> Buyurtma(int? beruvId, int? bronId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.BeruvId = beruvId;
            ViewBag.BronId = bronId;

            // Standart manzilni ko'rsatish
            var user = await _context.Users.FindAsync(userId);
            ViewBag.DefaultManzil = user?.Manzil;
            ViewBag.DefaultTelefon = user?.PhoneNumber ?? user?.Telefon;

            // Abonement bepul yetkazish tekshirish
            var faolAbonement = await _context.Abonementlar
                .Where(a => a.FoydalanuvchiId == userId && a.Holati == "Faol" && a.TugashSana > DateTime.Now)
                .Include(a => a.AbonementTuri)
                .FirstOrDefaultAsync();
            ViewBag.BepulYetkazish = faolAbonement?.AbonementTuri?.BepulYetkazish ?? false;
            ViewBag.YetkazishNarxi = (faolAbonement?.AbonementTuri?.BepulYetkazish ?? false) ? 0 : 5000;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buyurtma(YetkazibBerish model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.FoydalanuvchiId = userId!;
            model.BuyurtmaSana = DateTime.Now;
            model.YetkazilishKutilganSana = DateTime.Now.AddDays(1);
            model.KuzatuvRaqami = Guid.NewGuid().ToString()[..8].ToUpper();

            // Bepul yetkazish tekshirish
            var faolAbonement = await _context.Abonementlar
                .Where(a => a.FoydalanuvchiId == userId && a.Holati == "Faol" && a.TugashSana > DateTime.Now)
                .Include(a => a.AbonementTuri)
                .FirstOrDefaultAsync();

            if (faolAbonement?.AbonementTuri?.BepulYetkazish == true)
                model.YetkazishNarxi = 0;

            ModelState.Remove("FoydalanuvchiId");
            if (!ModelState.IsValid)
                return View(model);

            _context.YetkazibBerishlar.Add(model);
            await _context.SaveChangesAsync();

            await _bildirishnoma.YuborAsync(
                userId!,
                "🚚 Yetkazib berish buyurtmasi qabul qilindi",
                $"Buyurtmangiz qabul qilindi. Kuzatuv raqami: #{model.KuzatuvRaqami}. " +
                $"Manzil: {model.Manzil}. " +
                $"Taxminiy yetkazish: {model.YetkazilishKutilganSana:dd.MM.yyyy}.",
                "Yetkazish"
            );

            TempData["Xabar"] = $"Yetkazib berish buyurtmasi qabul qilindi! Kuzatuv: #{model.KuzatuvRaqami}";
            TempData["XabarTuri"] = "success";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Kuzatish(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var yetkazish = await _context.YetkazibBerishlar
                .Include(y => y.Beruv)
                    .ThenInclude(b => b!.KitobNusxa)
                        .ThenInclude(kn => kn!.Kitob)
                .FirstOrDefaultAsync(y => y.Id == id && y.FoydalanuvchiId == userId);

            if (yetkazish == null) return NotFound();
            return View(yetkazish);
        }
    }
}
