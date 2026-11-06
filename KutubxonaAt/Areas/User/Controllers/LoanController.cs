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
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AutoBerishService _autoBerishService;
        private const decimal DailyFine = LibraryRules.DailyFineAmount;

        public LoanController(ApplicationDbContext context, AutoBerishService autoBerishService)
        {
            _context = context;
            _autoBerishService = autoBerishService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loans = await _context.Beruvlar
                .Include(b => b.KitobNusxa).ThenInclude(kn => kn!.Kitob)
                .Where(b => b.FoydalanuvchiId == userId
                    && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan" || b.Holati == "MuddatiO'tgan"))
                .OrderByDescending(b => b.BerilganSana)
                .ToListAsync();

            var loanViewModels = loans.Select(loan => new LoanViewModel
            {
                Id = loan.Id,
                KitobNomi = loan.KitobNusxa?.Kitob?.Nomi ?? "Noma'lum",
                KitobRasm = loan.KitobNusxa?.Kitob?.Rasm,
                InventarRaqami = loan.KitobNusxa?.InventarRaqami ?? "",
                BerilganSana = loan.BerilganSana,
                QaytarishMuddati = loan.QaytarishMuddati,
                Holati = loan.Holati,
                UzaytirishlarSoni = loan.UzaytirishlarSoni,
                MaksUzaytirish = 3,
                FineAmount = loan.QaytarishMuddati < DateTime.Now
                    ? (int)(DateTime.Now - loan.QaytarishMuddati).TotalDays * DailyFine
                    : 0
            }).ToList();

            return View(loanViewModels);
        }

        // Barcha tarix
        public async Task<IActionResult> Tarix()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loans = await _context.Beruvlar
                .Include(b => b.KitobNusxa).ThenInclude(kn => kn!.Kitob)
                .Where(b => b.FoydalanuvchiId == userId)
                .OrderByDescending(b => b.BerilganSana)
                .ToListAsync();

            return View(loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loan = await _context.Beruvlar
                .Include(b => b.KitobNusxa)
                    .ThenInclude(kn => kn!.Kitob)
                .FirstOrDefaultAsync(b => b.Id == id && b.FoydalanuvchiId == userId);

            if (loan == null) return NotFound();

            if (loan.Holati == "Qaytarilgan")
            {
                TempData["Xabar"] = "Bu kitob allaqachon qaytarilgan.";
                TempData["XabarTuri"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            // Kechikish bo'lsa jarima
            if (loan.QaytarishMuddati < DateTime.Now)
            {
                int daysLate = (int)(DateTime.Now - loan.QaytarishMuddati).TotalDays;
                decimal fineAmount = daysLate * DailyFine;
                _context.Jarimalar.Add(new Jarima
                {
                    FoydalanuvchiId = userId!,
                    BeruvId = loan.Id,
                    Miqdor = fineAmount,
                    Valyuta = "UZS",
                    JarimaTuri = "Kechiktirilgan",
                    Sabab = $"Kitob {daysLate} kun kechiktirilgan",
                    BerilganSana = DateTime.Now,
                    Holati = "Kutilmoqda",
                    Izoh = "Avtomatik hisoblandi"
                });
                TempData["Ogoh"] = $"Kitob {daysLate} kun kechiktirilgan. {fineAmount:N0} so'm jarima qo'shildi.";
            }

            var nusxa = loan.KitobNusxa!;
            nusxa.Holati = "Mavjud";

            // Kitob mavjud nusxalar sonini oshirish
            if (nusxa.Kitob != null)
                nusxa.Kitob.MavjudNusxalar++;

            loan.QaytarilganSana = DateTime.Now;
            loan.Holati = "Qaytarilgan";
            await _context.SaveChangesAsync();

            // Kutish ro'yxatidagi birinchi kishiga bildiruv
            if (nusxa.KitobId > 0)
                await _autoBerishService.KutishRoyxatiUchunBildirishAsync(nusxa.KitobId);

            TempData["Xabar"] = "Kitob muvaffaqiyatli qaytarildi.";
            TempData["XabarTuri"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var natija = await _autoBerishService.UzaytirishAsync(id, userId!);

            TempData["Xabar"] = natija.Xabar;
            TempData["XabarTuri"] = natija.Muvaffaqiyatli ? "success" : "danger";
            return RedirectToAction(nameof(Index));
        }
    }

    public class LoanViewModel
    {
        public int Id { get; set; }
        public string KitobNomi { get; set; } = string.Empty;
        public string? KitobRasm { get; set; }
        public string InventarRaqami { get; set; } = string.Empty;
        public DateTime BerilganSana { get; set; }
        public DateTime QaytarishMuddati { get; set; }
        public string Holati { get; set; } = string.Empty;
        public int UzaytirishlarSoni { get; set; }
        public int MaksUzaytirish { get; set; }
        public decimal FineAmount { get; set; }
        public bool MuddatiOtganmi => QaytarishMuddati < DateTime.Now;
        public bool UzaytirishMumkinmi => !MuddatiOtganmi && UzaytirishlarSoni < MaksUzaytirish && FineAmount == 0;
        public int QolganKunlar => MuddatiOtganmi ? 0 : (int)(QaytarishMuddati - DateTime.Now).TotalDays;
    }
}
