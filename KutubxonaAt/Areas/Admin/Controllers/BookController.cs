using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.ViewModels.Kitob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Book
        public async Task<IActionResult> Index()
        {
            var books = await _context.Kitoblar
                .Include(b => b.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(b => b.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .ToListAsync();
            return View(books);
        }

        // GET: Admin/Book/Create
        public IActionResult Create()
        {
            var model = new KitobCrudViewModel();
            LoadSelectLists(model);
            return View(model);
        }

        // POST: Admin/Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KitobCrudViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kitob = new Kitob
                {
                    Nomi = model.Nomi,
                    AslNomi = model.AslNomi,
                    ISBN = model.ISBN,
                    MediaTuri = model.MediaTuri,
                    Tavsif = model.Tavsif,
                    Annotatsiya = model.Annotatsiya,
                    NashrYili = model.NashrYili,
                    SahifalarSoni = model.SahifalarSoni,
                    Nashriyot = model.Nashriyot,
                    Shahar = model.Shahar,
                    Til = model.Til,
                    UDK = model.UDK,
                    BBK = model.BBK,
                    MuallifBelgisi = model.MuallifBelgisi,
                    Narxi = model.Narxi,
                    NusxalarSoni = model.NusxalarSoni,
                    MavjudNusxalar = model.NusxalarSoni,
                    // Elektron maydon endi ishlatilmaydi, faqat MediaTuri asosida
                    Elektron = model.MediaTuri != MediaType.Kitob,
                    ElektronManzil = null,
                    FaylFormati = null
                };

                // Rasm yuklash (ixtiyoriy)
                if (model.RasmFayl != null && model.RasmFayl.Length > 0)
                {
                    if (!IsValidImageFile(model.RasmFayl))
                    {
                        ModelState.AddModelError("RasmFayl", "Rasm uchun faqat JPG, PNG, GIF, WEBP formatlar ruxsat etilgan (max 50MB).");
                        LoadSelectLists(model);
                        return View(model);
                    }
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/books");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.RasmFayl.FileName).ToLowerInvariant();
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.RasmFayl.CopyToAsync(fileStream);
                    }
                    kitob.Rasm = "/uploads/books/" + uniqueFileName;
                }

                // Raqamli fayl yuklash (faqat jismoniy kitob bo'lmagan taqdirda)
                if (model.MediaTuri != MediaType.Kitob && model.ElektronFayl != null && model.ElektronFayl.Length > 0)
                {
                    if (!IsValidMediaFile(model.ElektronFayl))
                    {
                        ModelState.AddModelError("ElektronFayl", "Media uchun faqat PDF, EPUB, MP3, MP4, AVI formatlar ruxsat etilgan (max 50MB).");
                        LoadSelectLists(model);
                        return View(model);
                    }
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/media");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ElektronFayl.FileName).ToLowerInvariant();
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ElektronFayl.CopyToAsync(fileStream);
                    }
                    kitob.ElektronManzil = "/uploads/media/" + uniqueFileName;
                    kitob.FaylFormati = Path.GetExtension(model.ElektronFayl.FileName).TrimStart('.').ToLowerInvariant();
                }

                _context.Kitoblar.Add(kitob);
                await _context.SaveChangesAsync();

                // Mualliflar
                foreach (var authorId in model.SelectedAuthorIds)
                {
                    _context.KitobMualliflari.Add(new KitobMuallif
                    {
                        KitobId = kitob.Id,
                        MuallifId = authorId
                    });
                }

                // Kategoriyalar
                foreach (var catId in model.SelectedCategoryIds)
                {
                    _context.KitobKategoriyalari.Add(new KitobKategoriya
                    {
                        KitobId = kitob.Id,
                        KategoriyaId = catId
                    });
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Kitob muvaffaqiyatli qo‘shildi!";
                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists(model);
            return View(model);
        }

        // GET: Admin/Book/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var kitob = await _context.Kitoblar
                .Include(b => b.KitobMualliflari)
                .Include(b => b.KitobKategoriyalari)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (kitob == null) return NotFound();

            var model = new KitobCrudViewModel
            {
                Id = kitob.Id,
                Nomi = kitob.Nomi,
                AslNomi = kitob.AslNomi,
                ISBN = kitob.ISBN,
                MediaTuri = kitob.MediaTuri,
                Tavsif = kitob.Tavsif,
                Annotatsiya = kitob.Annotatsiya,
                NashrYili = kitob.NashrYili,
                SahifalarSoni = kitob.SahifalarSoni,
                Nashriyot = kitob.Nashriyot,
                Shahar = kitob.Shahar,
                Til = kitob.Til,
                UDK = kitob.UDK,
                BBK = kitob.BBK,
                MuallifBelgisi = kitob.MuallifBelgisi,
                Narxi = kitob.Narxi,
                NusxalarSoni = kitob.NusxalarSoni,
                Rasm = kitob.Rasm,
                ElektronManzil = kitob.ElektronManzil,
                FaylFormati = kitob.FaylFormati,
                SelectedAuthorIds = kitob.KitobMualliflari.Select(km => km.MuallifId).ToList(),
                SelectedCategoryIds = kitob.KitobKategoriyalari.Select(kk => kk.KategoriyaId).ToList()
            };
            LoadSelectLists(model);
            return View(model);
        }

        // POST: Admin/Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KitobCrudViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var kitob = await _context.Kitoblar
                    .Include(b => b.KitobMualliflari)
                    .Include(b => b.KitobKategoriyalari)
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (kitob == null) return NotFound();

                // Asosiy ma'lumotlarni yangilash
                kitob.Nomi = model.Nomi;
                kitob.AslNomi = model.AslNomi;
                kitob.ISBN = model.ISBN;
                kitob.MediaTuri = model.MediaTuri;
                kitob.Tavsif = model.Tavsif;
                kitob.Annotatsiya = model.Annotatsiya;
                kitob.NashrYili = model.NashrYili;
                kitob.SahifalarSoni = model.SahifalarSoni;
                kitob.Nashriyot = model.Nashriyot;
                kitob.Shahar = model.Shahar;
                kitob.Til = model.Til;
                kitob.UDK = model.UDK;
                kitob.BBK = model.BBK;
                kitob.MuallifBelgisi = model.MuallifBelgisi;
                kitob.Narxi = model.Narxi;
                kitob.NusxalarSoni = model.NusxalarSoni;
                // Elektron maydonni MediaTuri asosida yangilash
                kitob.Elektron = model.MediaTuri != MediaType.Kitob;
                // Agar tur jismoniy bo'lsa, elektron faylni tozalash
                if (model.MediaTuri == MediaType.Kitob)
                {
                    // Agar eski elektron fayl mavjud bo'lsa, uni o'chirish
                    if (!string.IsNullOrEmpty(kitob.ElektronManzil))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, kitob.ElektronManzil.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    kitob.ElektronManzil = null;
                    kitob.FaylFormati = null;
                }

                // Rasm yangilash
                if (model.RasmFayl != null && model.RasmFayl.Length > 0)
                {
                    if (!IsValidImageFile(model.RasmFayl))
                    {
                        ModelState.AddModelError("RasmFayl", "Rasm uchun faqat JPG, PNG, GIF, WEBP formatlar ruxsat etilgan (max 50MB).");
                        LoadSelectLists(model);
                        return View(model);
                    }
                    if (!string.IsNullOrEmpty(kitob.Rasm))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, kitob.Rasm.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/books");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.RasmFayl.FileName).ToLowerInvariant();
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.RasmFayl.CopyToAsync(fileStream);
                    }
                    kitob.Rasm = "/uploads/books/" + uniqueFileName;
                }

                // Raqamli fayl yangilash (faqat jismoniy bo'lmagan va fayl yuklangan bo'lsa)
                if (model.MediaTuri != MediaType.Kitob && model.ElektronFayl != null && model.ElektronFayl.Length > 0)
                {
                    if (!IsValidMediaFile(model.ElektronFayl))
                    {
                        ModelState.AddModelError("ElektronFayl", "Media uchun faqat PDF, EPUB, MP3, MP4, AVI formatlar ruxsat etilgan (max 50MB).");
                        LoadSelectLists(model);
                        return View(model);
                    }
                    if (!string.IsNullOrEmpty(kitob.ElektronManzil))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, kitob.ElektronManzil.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/media");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ElektronFayl.FileName).ToLowerInvariant();
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ElektronFayl.CopyToAsync(fileStream);
                    }
                    kitob.ElektronManzil = "/uploads/media/" + uniqueFileName;
                    kitob.FaylFormati = Path.GetExtension(model.ElektronFayl.FileName).TrimStart('.').ToLowerInvariant();
                }

                // Mualliflarni yangilash
                _context.KitobMualliflari.RemoveRange(kitob.KitobMualliflari);
                foreach (var authorId in model.SelectedAuthorIds)
                {
                    _context.KitobMualliflari.Add(new KitobMuallif
                    {
                        KitobId = kitob.Id,
                        MuallifId = authorId
                    });
                }

                // Kategoriyalarni yangilash
                _context.KitobKategoriyalari.RemoveRange(kitob.KitobKategoriyalari);
                foreach (var catId in model.SelectedCategoryIds)
                {
                    _context.KitobKategoriyalari.Add(new KitobKategoriya
                    {
                        KitobId = kitob.Id,
                        KategoriyaId = catId
                    });
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Kitob tahrirlandi!";
                return RedirectToAction(nameof(Index));
            }

            LoadSelectLists(model);
            return View(model);
        }

        // POST: Admin/Book/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kitob = await _context.Kitoblar
                .Include(b => b.KitobNusxalari)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (kitob == null) return NotFound();

            if (kitob.KitobNusxalari.Any())
            {
                TempData["Error"] = "Bu kitobga bog‘langan nusxalar mavjud. Avval ularni o‘chiring.";
                return RedirectToAction(nameof(Index));
            }

            // Rasm faylini o‘chirish
            if (!string.IsNullOrEmpty(kitob.Rasm))
            {
                var rasmPath = Path.Combine(_webHostEnvironment.WebRootPath, kitob.Rasm.TrimStart('/'));
                if (System.IO.File.Exists(rasmPath))
                    System.IO.File.Delete(rasmPath);
            }

            // Elektron faylni o‘chirish
            if (!string.IsNullOrEmpty(kitob.ElektronManzil))
            {
                var ebookPath = Path.Combine(_webHostEnvironment.WebRootPath, kitob.ElektronManzil.TrimStart('/'));
                if (System.IO.File.Exists(ebookPath))
                    System.IO.File.Delete(ebookPath);
            }

            _context.Kitoblar.Remove(kitob);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kitob o‘chirildi!";
            return RedirectToAction(nameof(Index));
        }

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] AllowedMediaExtensions = { ".pdf", ".epub", ".mp3", ".mp4", ".avi" };
        private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

        private bool IsValidImageFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedImageExtensions.Contains(ext) && file.Length <= MaxFileSizeBytes;
        }

        private bool IsValidMediaFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedMediaExtensions.Contains(ext) && file.Length <= MaxFileSizeBytes;
        }

        private void LoadSelectLists(KitobCrudViewModel model)
        {
            model.AvailableAuthors = _context.Mualliflar
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Familiya + " " + a.Ism
                }).ToList();

            model.AvailableCategories = _context.Kategoriyalar
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nomi
                }).ToList();
        }
    }
}