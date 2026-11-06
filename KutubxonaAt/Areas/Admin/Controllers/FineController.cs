using KutubxonaAt.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Fine
        public async Task<IActionResult> Index(string status = "pending")
        {
            var query = _context.Jarimalar
                .Include(j => j.Foydalanuvchi)
                .Include(j => j.Beruv!)
                    .ThenInclude(b => b.KitobNusxa!)
                        .ThenInclude(kn => kn.Kitob)
                .AsQueryable();

            // Filter by status
            if (status == "pending")
                query = query.Where(j => j.Holati == "Kutilmoqda");
            else if (status == "paid")
                query = query.Where(j => j.Holati == "To'langan");
            else if (status == "cancelled")
                query = query.Where(j => j.Holati == "Bekor qilingan");
            else
                status = "pending";

            var fines = await query.OrderByDescending(j => j.BerilganSana).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(fines);
        }

        // POST: Admin/Fine/Pay/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id, string paymentMethod, string? notes = null)
        {
            var fine = await _context.Jarimalar.FindAsync(id);
            if (fine == null) return NotFound();

            if (fine.Holati != "Kutilmoqda")
            {
                TempData["Error"] = "Bu jarima allaqachon to‘langan yoki bekor qilingan.";
                return RedirectToAction(nameof(Index));
            }

            fine.Holati = "To'langan";
            fine.TolanganSana = DateTime.Now;
            fine.TolovUsuli = paymentMethod;
            if (!string.IsNullOrEmpty(notes))
                fine.Izoh = notes + (fine.Izoh != null ? "\n" + fine.Izoh : "");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Jarima to‘landi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Fine/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? reason = null)
        {
            var fine = await _context.Jarimalar.FindAsync(id);
            if (fine == null) return NotFound();

            if (fine.Holati != "Kutilmoqda")
            {
                TempData["Error"] = "Bu jarima allaqachon to‘langan yoki bekor qilingan.";
                return RedirectToAction(nameof(Index));
            }

            fine.Holati = "Bekor qilingan";
            if (!string.IsNullOrEmpty(reason))
                fine.Izoh = reason + (fine.Izoh != null ? "\n" + fine.Izoh : "");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Jarima bekor qilindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}