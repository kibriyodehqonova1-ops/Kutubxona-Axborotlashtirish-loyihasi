using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.ViewModels.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index(string search = "")
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u =>
                    u.Email!.Contains(search) ||
                    u.Ism.Contains(search) ||
                    u.Familiya.Contains(search) ||
                    u.TalabaId!.Contains(search));
            }

            var userList = await users.ToListAsync();
            var model = new List<UserIndexViewModel>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserIndexViewModel
                {
                    Id = user.Id,
                    Ism = user.Ism,
                    Familiya = user.Familiya,
                    Email = user.Email!,
                    Telefon = user.Telefon,
                    Turi = user.Turi,
                    Faol = user.Faol,
                    AmalMuddati = user.AmalMuddati,
                    Roles = string.Join(", ", roles)
                });
            }

            ViewBag.Search = search;
            return View(model);
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToListAsync();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Ism = user.Ism,
                Familiya = user.Familiya,
                Sharif = user.Sharif,
                Email = user.Email!,
                Telefon = user.Telefon,
                Manzil = user.Manzil,
                TugilganSana = user.TugilganSana,
                Turi = user.Turi,
                Fakultet = user.Fakultet,
                Guruh = user.Guruh,
                TalabaId = user.TalabaId,
                Faol = user.Faol,
                AmalMuddati = user.AmalMuddati,
                SelectedRoles = roles.ToList(),
                AllRoles = allRoles
            };
            return View(model);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                // Update basic info
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
                user.Faol = model.Faol;
                user.AmalMuddati = model.AmalMuddati;

                // Update email if changed
                if (user.Email != model.Email)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email; // username odatda email
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    await LoadRolesForEdit(model);
                    return View(model);
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();
                var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();

                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        await LoadRolesForEdit(model);
                        return View(model);
                    }
                }
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        foreach (var error in addResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        await LoadRolesForEdit(model);
                        return View(model);
                    }
                }

                TempData["Success"] = "Foydalanuvchi maʼlumotlari tahrirlandi.";
                return RedirectToAction(nameof(Index));
            }

            await LoadRolesForEdit(model);
            return View(model);
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Check if user has active loans (now using string comparison)
            var activeLoans = await _context.Beruvlar
                .AnyAsync(b => b.FoydalanuvchiId == id && b.Holati == "Berilgan");
            if (activeLoans)
            {
                TempData["Error"] = "Foydalanuvchining qaytarilmagan kitoblari bor. Avval ularni qaytarish kerak.";
                return RedirectToAction(nameof(Index));
            }

            // Check if user has active reservations
            var activeReservations = await _context.Bronlar
                .AnyAsync(b => b.FoydalanuvchiId == id && b.Holati == "Faol");
            if (activeReservations)
            {
                TempData["Error"] = "Foydalanuvchining faol bronlari mavjud. Avval ularni bekor qilish kerak.";
                return RedirectToAction(nameof(Index));
            }

            // Check if user has pending fines
            var pendingFines = await _context.Jarimalar
                .AnyAsync(f => f.FoydalanuvchiId == id && f.Holati == "Kutilmoqda");
            if (pendingFines)
            {
                TempData["Error"] = "Foydalanuvchining toʻlanmagan jarimalari bor. Avval ularni toʻlash kerak.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Foydalanuvchi oʻchirildi.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    TempData["Error"] = error.Description;
                    break;
                }
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task LoadRolesForEdit(UserEditViewModel model)
        {
            var allRoles = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToListAsync();
            model.AllRoles = allRoles;
        }
    }
}