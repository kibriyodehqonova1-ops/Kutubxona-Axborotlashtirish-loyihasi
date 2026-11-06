using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Kategoriyalar
                .Include(k => k.OtaKategoriya)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            ViewBag.ParentCategories = _context.Kategoriyalar
                .Select(k => new { k.Id, k.Nomi })
                .ToList();
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kategoriya kategoriya)
        {
            if (ModelState.IsValid)
            {
                _context.Kategoriyalar.Add(kategoriya);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategoriya muvaffaqiyatli qo‘shildi!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ParentCategories = _context.Kategoriyalar
                .Select(k => new { k.Id, k.Nomi })
                .ToList();
            return View(kategoriya);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var kategoriya = await _context.Kategoriyalar.FindAsync(id);
            if (kategoriya == null)
                return NotFound();

            ViewBag.ParentCategories = _context.Kategoriyalar
                .Where(k => k.Id != id)
                .Select(k => new { k.Id, k.Nomi })
                .ToList();
            return View(kategoriya);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Kategoriya kategoriya)
        {
            if (id != kategoriya.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategoriya);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kategoriya tahrirlandi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Kategoriyalar.Any(k => k.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ParentCategories = _context.Kategoriyalar
                .Where(k => k.Id != id)
                .Select(k => new { k.Id, k.Nomi })
                .ToList();
            return View(kategoriya);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kategoriya = await _context.Kategoriyalar.FindAsync(id);
            if (kategoriya != null)
            {
                _context.Kategoriyalar.Remove(kategoriya);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategoriya o‘chirildi!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}