using KutubxonaAt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Librarian")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Asosiy statistika
            ViewBag.BooksCount = await _context.Kitoblar.CountAsync();
            ViewBag.IssuedCount = await _context.Beruvlar.CountAsync(b => b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan");
            ViewBag.ReturnedCount = await _context.Beruvlar.CountAsync(b => b.Holati == "Qaytarilgan");
            ViewBag.UsersCount = await _context.Users.CountAsync();
            ViewBag.AuthorsCount = await _context.Mualliflar.CountAsync();
            ViewBag.CategoriesCount = await _context.Kategoriyalar.CountAsync();

            // Qo'shimcha statistika
            ViewBag.OverdueCount = await _context.Beruvlar.CountAsync(
                b => (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan") && b.QaytarishMuddati < DateTime.Now);
            ViewBag.PendingFinesCount = await _context.Jarimalar.CountAsync(j => j.Holati == "Kutilmoqda");
            ViewBag.ActiveReservationsCount = await _context.Bronlar.CountAsync(b => b.Holati == "Faol");
            ViewBag.PendingOrdersCount = await _context.Orders.CountAsync(o => o.Holati == "Kutilmoqda");
            ViewBag.TotalFinesAmount = await _context.Jarimalar
                .Where(j => j.Holati == "Kutilmoqda")
                .SumAsync(j => (decimal?)j.Miqdor) ?? 0;

            // So'nggi 5 ta beruv
            ViewBag.RecentBeruvlar = await _context.Beruvlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.KitobNusxa!).ThenInclude(kn => kn.Kitob)
                .OrderByDescending(b => b.BerilganSana)
                .Take(5)
                .ToListAsync();

            // So'nggi 5 ta ro'yxatdan o'tgan foydalanuvchi
            ViewBag.RecentUsers = await _context.Users
                .OrderByDescending(u => u.RoyxatSanasi)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}