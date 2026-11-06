using KutubxonaAt.Data;
using KutubxonaAt.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Author/Index
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Mualliflar.ToListAsync();
            return View(authors);
        }

        // GET: /Admin/Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Muallif muallif)
        {
            if (ModelState.IsValid)
            {
                _context.Mualliflar.Add(muallif);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Muallif muvaffaqiyatli qo‘shildi!";
                return RedirectToAction(nameof(Index));
            }
            return View(muallif);
        }

        // GET: /Admin/Author/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var muallif = await _context.Mualliflar.FindAsync(id);
            if (muallif == null)
                return NotFound();
            return View(muallif);
        }

        // POST: /Admin/Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Muallif muallif)
        {
            if (id != muallif.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(muallif);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Muallif tahrirlandi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Mualliflar.Any(m => m.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(muallif);
        }

        // POST: /Admin/Author/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var muallif = await _context.Mualliflar.FindAsync(id);
            if (muallif != null)
            {
                // Agar muallifga bog‘liq kitoblar mavjud bo‘lsa, o‘chirishni oldini olish
                bool hasBooks = await _context.KitobMualliflari.AnyAsync(km => km.MuallifId == id);
                if (hasBooks)
                {
                    TempData["Error"] = "Bu muallifga tegishli kitoblar mavjud. Avval ularni o‘chiring.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Mualliflar.Remove(muallif);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Muallif o‘chirildi!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}