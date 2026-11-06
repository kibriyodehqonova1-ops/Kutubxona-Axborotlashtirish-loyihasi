using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BeruvController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BeruvController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Beruv – barcha beruvlar (berilgan, qaytarilgan)
        public async Task<IActionResult> Index(string status = "all")
        {
            var query = _context.Beruvlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.KitobNusxa!).ThenInclude(kn => kn.Kitob)
                .AsQueryable();

            if (status == "active")
                query = query.Where(b => b.Holati == "Berilgan");
            else if (status == "returned")
                query = query.Where(b => b.Holati == "Qaytarilgan");
            else if (status == "overdue")
                query = query.Where(b => b.Holati == "Berilgan" && b.QaytarishMuddati < DateTime.Now);

            var beruvlar = await query.OrderByDescending(b => b.BerilganSana).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(beruvlar);
        }

        // GET: Admin/Beruv/IssueFromBron – faol bronlar ro‘yxati
        public async Task<IActionResult> IssueFromBron()
        {
            var activeBrons = await _context.Bronlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.Kitob)
                .Include(b => b.KitobNusxa)
                .Where(b => b.Holati == "Faol")
                .OrderBy(b => b.BronSana)
                .ToListAsync();
            return View(activeBrons);
        }

        // POST: Admin/Beruv/IssueFromBron/5 – bron asosida kitob berish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueFromBron(int bronId)
        {
            var bron = await _context.Bronlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.KitobNusxa!).ThenInclude(kn => kn.Kitob)
                .FirstOrDefaultAsync(b => b.Id == bronId);
            if (bron == null) return NotFound();

            if (bron.Holati != "Faol")
            {
                TempData["Error"] = "Bu bron allaqachon bekor qilingan yoki bajarilgan.";
                return RedirectToAction(nameof(IssueFromBron));
            }

            // Nusxa tekshiruvi
            var nusxa = bron.KitobNusxa;
            if (nusxa == null || nusxa.Holati != "Mavjud")
            {
                TempData["Error"] = "Kitob nusxasi mavjud emas (allaqachon berilgan yoki yo‘qolgan).";
                return RedirectToAction(nameof(IssueFromBron));
            }

            var user = bron.Foydalanuvchi;
            if (user == null || !user.Faol)
            {
                TempData["Error"] = "Foydalanuvchi topilmadi yoki faol emas.";
                return RedirectToAction(nameof(IssueFromBron));
            }
            if (user.AmalMuddati < DateTime.Now)
            {
                TempData["Error"] = "Foydalanuvchining abonement muddati tugagan.";
                return RedirectToAction(nameof(IssueFromBron));
            }

            var pendingFines = await _context.Jarimalar.AnyAsync(f => f.FoydalanuvchiId == user.Id && f.Holati == "Kutilmoqda");
            if (pendingFines)
            {
                TempData["Error"] = "Foydalanuvchining to‘lanmagan jarimalari bor.";
                return RedirectToAction(nameof(IssueFromBron));
            }

            var activeLoans = await _context.Beruvlar.CountAsync(b => b.FoydalanuvchiId == user.Id && b.Holati == "Berilgan");
            if (activeLoans >= 5)
            {
                TempData["Error"] = "Foydalanuvchi 5 tadan ortiq kitob ololmaydi.";
                return RedirectToAction(nameof(IssueFromBron));
            }

            // Beruv yozuvini yaratish
            var beruv = new Beruv
            {
                FoydalanuvchiId = user.Id,
                KitobNusxaId = nusxa.Id,
                BerilganSana = DateTime.Now,
                QaytarishMuddati = DateTime.Now.AddDays(30),
                Holati = "Berilgan",
                BerganXodim = User.Identity?.Name,
                Izoh = $"Bron asosida berildi. Bron ID: {bron.Id}"
            };

            // Nusxa holatini o‘zgartirish
            nusxa.Holati = "Berilgan";
            if (nusxa.Kitob != null && nusxa.Kitob.MavjudNusxalar > 0)
                nusxa.Kitob.MavjudNusxalar--;

            // Bronni bajarilgan deb belgilash
            bron.Holati = "Bajarilgan";

            _context.Beruvlar.Add(beruv);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Kitob muvaffaqiyatli berildi. Beruv raqami: {beruv.BeruvRaqami}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Beruv/Return/5 – kitob qaytarish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, string? izoh = null, string? condition = null)
        {
            var beruv = await _context.Beruvlar
                .Include(b => b.KitobNusxa!).ThenInclude(kn => kn.Kitob)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (beruv == null) return NotFound();

            if (beruv.Holati != "Berilgan")
            {
                TempData["Error"] = "Bu kitob allaqachon qaytarilgan.";
                return RedirectToAction(nameof(Index));
            }

            beruv.QaytarilganSana = DateTime.Now;
            beruv.Holati = "Qaytarilgan";
            beruv.QabulQilganXodim = User.Identity?.Name;
            if (!string.IsNullOrEmpty(izoh)) beruv.Izoh = izoh;

            var nusxa = beruv.KitobNusxa;
            if (nusxa != null)
            {
                nusxa.Holati = "Mavjud";
                if (!string.IsNullOrEmpty(condition)) nusxa.JismoniyHolati = condition;
                if (nusxa.Kitob != null) nusxa.Kitob.MavjudNusxalar++;
            }

            // Kechiktirilganlik jarimasi
            if (beruv.QaytarilganSana > beruv.QaytarishMuddati)
            {
                int overdueDays = (beruv.QaytarilganSana.Value - beruv.QaytarishMuddati).Days;
                decimal fineAmount = overdueDays * KutubxonaAt.Services.LibraryRules.DailyFineAmount;
                var jarima = new Jarima
                {
                    FoydalanuvchiId = beruv.FoydalanuvchiId,
                    BeruvId = beruv.Id,
                    Miqdor = fineAmount,
                    JarimaTuri = "Kechiktirilgan",
                    Sabab = $"{overdueDays} kun kechiktirilgan",
                    BerilganSana = DateTime.Now,
                    Holati = "Kutilmoqda"
                };
                _context.Jarimalar.Add(jarima);
                TempData["Warning"] = $"Kitob {overdueDays} kun kech qaytarildi. {fineAmount:N0} so‘m jarima hisoblandi.";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Kitob qaytarildi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Beruv/Renew/5 – muddat uzaytirish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id)
        {
            var beruv = await _context.Beruvlar.FindAsync(id);
            if (beruv == null) return NotFound();
            if (beruv.Holati != "Berilgan")
            {
                TempData["Error"] = "Bu kitobni uzaytirish mumkin emas.";
                return RedirectToAction(nameof(Index));
            }
            if (beruv.UzaytirishlarSoni >= 3)
            {
                TempData["Error"] = "Kitobni 3 martadan ortiq uzaytirish mumkin emas.";
                return RedirectToAction(nameof(Index));
            }
            beruv.UzaytirishlarSoni++;
            beruv.UzaytirilganMuddat = beruv.QaytarishMuddati.AddDays(15);
            beruv.QaytarishMuddati = beruv.UzaytirilganMuddat.Value;
            beruv.Holati = "Uzaytirilgan";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kitob muddati 15 kunga uzaytirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}