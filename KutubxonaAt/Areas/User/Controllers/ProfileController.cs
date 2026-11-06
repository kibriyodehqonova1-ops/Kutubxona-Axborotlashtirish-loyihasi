using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.ViewModels.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Ism = user.Ism,
                Familiya = user.Familiya,
                Sharif = user.Sharif,
                Email = user.Email ?? string.Empty,
                Telefon = user.Telefon,
                Manzil = user.Manzil,
                TugilganSana = user.TugilganSana,
                Turi = user.Turi,
                Fakultet = user.Fakultet,
                Guruh = user.Guruh,
                TalabaId = user.TalabaId,
                RoyxatSanasi = user.RoyxatSanasi,
                AmalMuddati = user.AmalMuddati,
                Faol = user.Faol
            };

            // So‘nggi bronlar
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.RecentReservations = await _context.Bronlar
                .Include(b => b.Kitob)
                .Where(b => b.FoydalanuvchiId == userId)
                .OrderByDescending(b => b.BronSana)
                .Take(5)
                .ToListAsync();

            //// Sotib olingan kitoblar
            //ViewBag.Purchases = await _context.Sotuvlar
            //    .Include(s => s.Kitob)
            //    .Where(s => s.FoydalanuvchiId == userId)
            //    .OrderByDescending(s => s.SotibOlinganSana)
            //    .Take(5)
            //    .ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Ism = model.Ism;
            user.Familiya = model.Familiya;
            user.Sharif = model.Sharif;
            user.Telefon = model.Telefon;
            user.Manzil = model.Manzil;
            user.TugilganSana = model.TugilganSana;
            user.Turi = model.Turi;
            user.Fakultet = model.Fakultet;
            user.Guruh = model.Guruh;
            user.TalabaId = model.TalabaId;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profil ma'lumotlari yangilandi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Parol muvaffaqiyatli o‘zgartirildi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}