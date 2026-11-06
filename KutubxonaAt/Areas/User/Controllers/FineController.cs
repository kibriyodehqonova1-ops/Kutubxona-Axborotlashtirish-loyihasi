using KutubxonaAt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class FineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fines = await _context.Jarimalar
                .Include(f => f.Beruv!).ThenInclude(b => b.KitobNusxa!).ThenInclude(kn => kn.Kitob)
                .Where(f => f.FoydalanuvchiId == userId)
                .OrderByDescending(f => f.BerilganSana)
                .ToListAsync();

            return View(fines);
        }
    }
}