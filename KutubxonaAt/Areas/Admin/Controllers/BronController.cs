using KutubxonaAt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BronController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BronController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Bron
        public async Task<IActionResult> Index(string status = "all")
        {
            var query = _context.Bronlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.Kitob)
                .AsQueryable();

            // Filter by status
            if (status == "active")
                query = query.Where(b => b.Holati == "Faol");
            else if (status == "cancelled")
                query = query.Where(b => b.Holati == "Bekor qilingan");
            else if (status == "completed")
                query = query.Where(b => b.Holati == "Bajarilgan");
            else
                status = "all";

            var bronlar = await query.OrderByDescending(b => b.BronSana).ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(bronlar);
        }

        // POST: Admin/Bron/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var bron = await _context.Bronlar.FindAsync(id);
            if (bron == null) return NotFound();

            // Faqat faol bronlarni bekor qilish mumkin
            if (bron.Holati != "Faol")
            {
                TempData["Error"] = "Bu bron allaqachon bekor qilingan yoki bajarilgan.";
                return RedirectToAction(nameof(Index));
            }

            bron.Holati = "Bekor qilingan";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bron bekor qilindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}