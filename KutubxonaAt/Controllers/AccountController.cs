using KutubxonaAt.Models;
using KutubxonaAt.Models.ViewModels.Account;
using KutubxonaAt.ViewModels.Account;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KutubxonaAt.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user!);

                    // returnUrl mavjud bo'lsa va xavfsiz bo'lsa, u yerga yo'naltir
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    else if (roles.Contains("Librarian"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    else
                        return RedirectToAction("Index", "Home", new { area = "User" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email yoki parol noto‘g‘ri.");
                    return View(model);
                }
            }
            return View(model);
        }
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Email bandligini tekshirish
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu email allaqachon ro'yxatdan o'tgan.");
                    return View(model);
                }

                // Yangi foydalanuvchi yaratish
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Ism = model.Ism,
                    Familiya = model.Familiya,
                    Sharif = model.Sharif,
                    Telefon = model.Telefon,
                    Manzil = model.Manzil,
                    TugilganSana = model.TugilganSana,
                    Turi = model.Turi ?? "Boshqa",
                    Fakultet = model.Fakultet,
                    Guruh = model.Guruh,
                    TalabaId = model.TalabaId,
                    RoyxatSanasi = DateTime.Now,
                    AmalMuddati = DateTime.Now.AddYears(1),
                    Faol = true
                };

                // Foydalanuvchini bazaga saqlash (parol avtomatik hash qilinadi)
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Foydalanuvchini "User" roliga qo'shish (agar rollar mavjud bo'lsa)
                    await _userManager.AddToRoleAsync(user, "User");

                    // Tizimga avtomatik kirish
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Muvaffaqiyat"] = "Ro'yxatdan muvaffaqiyatli o'tdingiz!";
                    return RedirectToAction("Index", "Home");
                }

                // Xatoliklar bo'lsa, foydalanuvchiga ko'rsatish
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        // GET: /Account/ForgotPassword
        [HttpGet]
       
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Xavfsizlik: foydalanuvchi topilmasa ham "email yuborildi" deymiz
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Parolni tiklash tokenini yaratish
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // XAVFSIZLIK: token hech qachon brauzerga qaytarilmasligi kerak, aks holda email manzilini
                // bilgan har kim (masalan admin) boshqa birovning parolini almashtira oladi.
                // Haqiqiy loyihada token linki foydalanuvchi emailiga yuboriladi (email xizmati hali ulanmagan).
                // Faqat lokal ishlab chiqish (Development) qulayligi uchun to'g'ridan-to'g'ri yo'naltiramiz.
                var env = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
                if (env.IsDevelopment())
                {
                    return RedirectToAction(nameof(ResetPassword), new { email = model.Email, token = token });
                }
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Email va token majburiy.");
            }
            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
    
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Foydalanuvchi topilmasa ham muvaffaqiyatli deb ko'rsatamiz (xavfsizlik)
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
     
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}