using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.ViewModels.BookCopy;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookCopyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookCopyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/BookCopy?bookId=5
        public async Task<IActionResult> Index(int? bookId)
        {
            // Agar bookId berilmagan bo‘lsa, barcha nusxalarni ko‘rsat
            if (!bookId.HasValue)
            {
                var allCopies = await _context.KitobNusxalari
                    .Include(kn => kn.Kitob)
                    .ToListAsync();
                return View(allCopies);
            }

            var book = await _context.Kitoblar.FindAsync(bookId);
            if (book == null) return NotFound();

            // To‘g‘ri enum ishlatish
            if (book.MediaTuri != Kitob.MediaType.Kitob)
            {
                TempData["Info"] = $"\"{book.Nomi}\" ({book.MediaTuri}) turdagi resurs uchun jismoniy nusxalar boshqarilmaydi.";
                return RedirectToAction("Index", "KitobNusxa"); // yoki o‘ziga qaytarsa ham bo‘ladi
            }

            var copies = await _context.KitobNusxalari
                .Where(kn => kn.KitobId == bookId)
                .Include(kn => kn.Kitob)
                .ToListAsync();

            ViewBag.BookId = bookId;
            ViewBag.BookTitle = book.Nomi;
            return View(copies);
        }

        // GET: Admin/BookCopy/Create?bookId=5
        public async Task<IActionResult> Create(int bookId)
        {
            var book = await _context.Kitoblar.FindAsync(bookId);
            if (book == null) return NotFound();

            if (book.MediaTuri != MediaType.Kitob)
            {
                TempData["Error"] = "Faqat jismoniy kitoblarga nusxa qo‘shish mumkin.";
                return RedirectToAction("Index", "Book");
            }

            var model = new BookCopyCreateViewModel
            {
                KitobId = bookId,
                KitobNomi = book.Nomi,
                QabulQilinganSana = DateTime.Now,
                Holati = "Mavjud",
                JismoniyHolati = "Yaxshi"
            };
            return View(model);
        }

        // POST: Admin/BookCopy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCopyCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Kitoblar.FindAsync(model.KitobId);
                if (book == null || book.MediaTuri != MediaType.Kitob)
                {
                    TempData["Error"] = "Faqat jismoniy kitoblarga nusxa qo‘shish mumkin.";
                    return RedirectToAction("Index", "Book");
                }

                // Agar inventar raqam berilmagan bo‘lsa, avtomatik yaratish
                if (string.IsNullOrEmpty(model.InventarRaqami))
                {
                    var lastCopy = await _context.KitobNusxalari
                        .OrderByDescending(kn => kn.Id)
                        .FirstOrDefaultAsync();
                    int nextId = (lastCopy?.Id ?? 0) + 1;
                    model.InventarRaqami = $"INV-{DateTime.Now.Year}-{nextId:D5}";
                }

                var copy = new KitobNusxa
                {
                    KitobId = model.KitobId,
                    InventarRaqami = model.InventarRaqami,
                    BarKod = model.BarKod,
                    RFID = model.RFID,
                    QabulQilinganSana = model.QabulQilinganSana,
                    KelibChiqishi = model.KelibChiqishi,
                    Narxi = model.Narxi,
                    Holati = model.Holati,
                    JismoniyHolati = model.JismoniyHolati,
                    Joylashuv = model.Joylashuv,
                    OquvZalidanChiqmaydi = model.OquvZalidanChiqmaydi,
                    Cheklangan = model.Cheklangan,
                    OxirgiInventarizatsiya = model.OxirgiInventarizatsiya,
                    Izoh = model.Izoh
                };

                _context.KitobNusxalari.Add(copy);
                await _context.SaveChangesAsync();

                // Kitobdagi nusxalar sonini yangilash
                await UpdateBookCounts(model.KitobId);

                TempData["Success"] = "Yangi nusxa qo‘shildi.";
                return RedirectToAction("Index", new { bookId = model.KitobId });
            }

            var book2 = await _context.Kitoblar.FindAsync(model.KitobId);
            model.KitobNomi = book2?.Nomi ?? "";
            return View(model);
        }

        // GET: Admin/BookCopy/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var copy = await _context.KitobNusxalari
                .Include(kn => kn.Kitob)
                .FirstOrDefaultAsync(kn => kn.Id == id);
            if (copy == null) return NotFound();

            if (copy.Kitob?.MediaTuri != MediaType.Kitob)
            {
                TempData["Error"] = "Bu nusxa jismoniy kitobga tegishli emas.";
                return RedirectToAction("Index", "Book");
            }

            var model = new BookCopyEditViewModel
            {
                Id = copy.Id,
                KitobId = copy.KitobId,
                KitobNomi = copy.Kitob?.Nomi,
                InventarRaqami = copy.InventarRaqami,
                BarKod = copy.BarKod,
                RFID = copy.RFID,
                QabulQilinganSana = copy.QabulQilinganSana,
                KelibChiqishi = copy.KelibChiqishi,
                Narxi = copy.Narxi,
                Holati = copy.Holati,
                JismoniyHolati = copy.JismoniyHolati,
                Joylashuv = copy.Joylashuv,
                OquvZalidanChiqmaydi = copy.OquvZalidanChiqmaydi,
                Cheklangan = copy.Cheklangan,
                OxirgiInventarizatsiya = copy.OxirgiInventarizatsiya,
                Izoh = copy.Izoh
            };

            await PopulateSelectLists();
            return View(model);
        }

        // POST: Admin/BookCopy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookCopyEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var copy = await _context.KitobNusxalari.FindAsync(id);
                if (copy == null) return NotFound();

                var book = await _context.Kitoblar.FindAsync(copy.KitobId);
                if (book?.MediaTuri != MediaType.Kitob)
                {
                    TempData["Error"] = "Faqat jismoniy kitob nusxalarini tahrirlash mumkin.";
                    return RedirectToAction("Index", "Book");
                }

                // Holatni “Berilgan” qilishda aktiv beruv mavjudligini tekshirish
                if (model.Holati == "Berilgan" && copy.Holati != "Berilgan")
                {
                    var activeLoan = await _context.Beruvlar
                        .AnyAsync(b => b.KitobNusxaId == id && b.Holati == "Berilgan");
                    if (activeLoan)
                    {
                        ModelState.AddModelError("Holati", "Bu nusxa hozirda berilgan. Holatni o‘zgartirish mumkin emas.");
                        await PopulateSelectLists();
                        return View(model);
                    }
                }

                copy.InventarRaqami = model.InventarRaqami;
                copy.BarKod = model.BarKod;
                copy.RFID = model.RFID;
                copy.QabulQilinganSana = model.QabulQilinganSana;
                copy.KelibChiqishi = model.KelibChiqishi;
                copy.Narxi = model.Narxi;
                copy.Holati = model.Holati;
                copy.JismoniyHolati = model.JismoniyHolati;
                copy.Joylashuv = model.Joylashuv;
                copy.OquvZalidanChiqmaydi = model.OquvZalidanChiqmaydi;
                copy.Cheklangan = model.Cheklangan;
                copy.OxirgiInventarizatsiya = model.OxirgiInventarizatsiya;
                copy.Izoh = model.Izoh;

                _context.Update(copy);
                await _context.SaveChangesAsync();

                await UpdateBookCounts(copy.KitobId);
                TempData["Success"] = "Nusxa tahrirlandi.";
                return RedirectToAction("Index", new { bookId = copy.KitobId });
            }

            await PopulateSelectLists();
            return View(model);
        }

        // POST: Admin/BookCopy/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var copy = await _context.KitobNusxalari
                .Include(kn => kn.Beruvlar)
                .FirstOrDefaultAsync(kn => kn.Id == id);
            if (copy == null) return NotFound();

            if (copy.Beruvlar.Any())
            {
                TempData["Error"] = "Bu nusxaga bog‘liq beruvlar mavjud. O‘chirish mumkin emas.";
                return RedirectToAction("Index", new { bookId = copy.KitobId });
            }

            var kitobId = copy.KitobId;
            _context.KitobNusxalari.Remove(copy);
            await _context.SaveChangesAsync();
            await UpdateBookCounts(kitobId);

            TempData["Success"] = "Nusxa o‘chirildi.";
            return RedirectToAction("Index", new { bookId = kitobId });
        }

        // POST: Admin/BookCopy/ToggleSale/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSale(int id)
        {
            var copy = await _context.KitobNusxalari.FindAsync(id);
            if (copy == null) return NotFound();

            // Faqat mavjud nusxani sotuvga qo'yish mumkin
            if (!copy.SotuvgaQoyilgan && copy.Holati != "Mavjud")
            {
                TempData["Error"] = "Faqat 'Mavjud' holatidagi nusxani sotuvga qo'yish mumkin.";
                return RedirectToAction("Index", new { bookId = copy.KitobId });
            }

            copy.SotuvgaQoyilgan = !copy.SotuvgaQoyilgan;
            _context.Update(copy);
            await _context.SaveChangesAsync();

            TempData["Success"] = copy.SotuvgaQoyilgan
                ? $"Nusxa #{copy.Id} sotuvga qo'yildi."
                : $"Nusxa #{copy.Id} sotuvdan olib tashlandi.";

            return RedirectToAction("Index", new { bookId = copy.KitobId });
        }

        private async Task UpdateBookCounts(int bookId)
        {
            var kitob = await _context.Kitoblar.FindAsync(bookId);
            if (kitob != null && kitob.MediaTuri == MediaType.Kitob)
            {
                kitob.NusxalarSoni = await _context.KitobNusxalari.CountAsync(kn => kn.KitobId == bookId);
                kitob.MavjudNusxalar = await _context.KitobNusxalari.CountAsync(kn => kn.KitobId == bookId && kn.Holati == "Mavjud");
                _context.Update(kitob);
                await _context.SaveChangesAsync();
            }
        }

        private Task PopulateSelectLists()
        {
            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Mavjud", Text = "Mavjud" },
                new SelectListItem { Value = "Berilgan", Text = "Berilgan" },
                new SelectListItem { Value = "Ta'mirda", Text = "Ta'mirda" },
                new SelectListItem { Value = "Yo'qolgan", Text = "Yo'qolgan" }
            };
            ViewBag.ConditionOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Yangi", Text = "Yangi" },
                new SelectListItem { Value = "Yaxshi", Text = "Yaxshi" },
                new SelectListItem { Value = "Eskirgan", Text = "Eskirgan" },
                new SelectListItem { Value = "Shikastlangan", Text = "Shikastlangan" }
            };
            return Task.CompletedTask;
        }
    }
}