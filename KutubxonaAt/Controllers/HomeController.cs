using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.Models.ViewModels;
using KutubxonaAt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 9;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string keywords, int? catalogId, int? categoryId)
        {
            var eBookCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.ElektronKitob);
            var audioBookCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.AudioKitob);
            var journalCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.Jurnal);
            var videoCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.Video);

            // So‘nggi 6 kitobni BookCardViewModel ga o‘tkazish
            var recentBooks = await _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .OrderByDescending(k => k.QoshilganSana)
                .Take(6)
                .Select(k => new BookCardViewModel
                {
                    Id = k.Id,
                    Nomi = k.Nomi,
                    Rasm = k.Rasm ?? "/images/default-book.jpg",
                    ISBN = k.ISBN ?? "Mavjud emas",
                    OrtachaBaho = k.OrtachaBaho,
                    MediaTuri = k.MediaTuri.ToString(),
                    Tavsif = k.Tavsif ?? "Tavsif mavjud emas",
                    Mualliflar = string.Join(", ", k.KitobMualliflari.Select(km => $"{km.Muallif.Familiya} {km.Muallif.Ism}")),
                    Kategoriyalar = string.Join(", ", k.KitobKategoriyalari.Select(kk => kk.Kategoriya.Nomi))
                })
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                LeadText = "Bilim va ma'rifat maskani",
                WelcomeText = "Kutubxonamiz barcha kitobxonlar uchun ochiq. 50 000 dan ortiq asarlar, zamonaviy katalog va qulay xizmatlar sizni kutmoqda.",
                EBookCount = eBookCount,
                AudioBookCount = audioBookCount,
                JournalCount = journalCount,
                VideoCount = videoCount,
                RecentBooks = recentBooks,
                IsSearch = false
            };

            bool isSearching = !string.IsNullOrEmpty(keywords) || catalogId.HasValue || categoryId.HasValue;

            if (isSearching)
            {
                var query = _context.Kitoblar
                    .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                    .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(k => k.Nomi.Contains(keywords) ||
                        k.KitobMualliflari.Any(km => km.Muallif.Ism.Contains(keywords) || km.Muallif.Familiya.Contains(keywords)) ||
                        k.KitobKategoriyalari.Any(kk => kk.Kategoriya.Nomi.Contains(keywords)));
                }

                if (catalogId.HasValue)
                {
                    MediaType mediaType = catalogId.Value switch
                    {
                        1 => MediaType.Kitob,
                        2 => MediaType.ElektronKitob,
                        3 => MediaType.AudioKitob,
                        4 => MediaType.Jurnal,
                        5 => MediaType.Video,
                        _ => MediaType.Kitob
                    };
                    query = query.Where(k => k.MediaTuri == mediaType);
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(k => k.KitobKategoriyalari.Any(kk => kk.KategoriyaId == categoryId.Value));
                }

                var results = await query
                    .Select(k => new BookCardViewModel
                    {
                        Id = k.Id,
                        Nomi = k.Nomi,
                        Rasm = k.Rasm ?? "/images/default-book.jpg",
                        ISBN = k.ISBN ?? "Mavjud emas",
                        OrtachaBaho = k.OrtachaBaho,
                        MediaTuri = k.MediaTuri.ToString(),
                        Tavsif = k.Tavsif ?? "Tavsif mavjud emas",
                        Mualliflar = string.Join(", ", k.KitobMualliflari.Select(km => $"{km.Muallif.Familiya} {km.Muallif.Ism}")),
                        Kategoriyalar = string.Join(", ", k.KitobKategoriyalari.Select(kk => kk.Kategoriya.Nomi))
                    })
                    .ToListAsync();

                viewModel.IsSearch = true;
                viewModel.SearchResults = results;
                viewModel.SearchKeyword = keywords ?? "Barcha";
                viewModel.SelectedCatalog = catalogId;
                viewModel.SelectedCategoryId = categoryId;
            }

            ViewBag.Categories = await _context.Kategoriyalar.ToListAsync();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.Keywords = keywords;
            ViewBag.SelectedCatalog = catalogId.HasValue ? catalogId.Value.ToString() : "";

            return View(viewModel);
        }
        private static string GetFilterClasses(MediaType mediaType, IEnumerable<string> categories)
        {
            var classes = new List<string>();

            // Media turi bo‘yicha
            switch (mediaType)
            {
                case MediaType.Video:
                    classes.Add("video");
                    break;
                case MediaType.AudioKitob:
                    classes.Add("audio");
                    break;
                case MediaType.Jurnal:
                    classes.Add("magazines");
                    break;
                case MediaType.Kitob:
                    classes.Add("books");
                    break;
                case MediaType.ElektronKitob:
                    classes.Add("ebooks");
                    break;
            }

            // Kategoriyalar bo‘yicha
            foreach (var cat in categories)
            {
                if (cat.Contains("Badiiy") || cat.Contains("adabiyot"))
                    classes.Add("adults");
                else if (cat.Contains("Bolalar") || cat.Contains("ertak"))
                    classes.Add("kids-teens");
                else if (cat.Contains("Ilmiy") || cat.Contains("fan"))
                    classes.Add("books");
            }

            if (classes.Count == 0)
                classes.Add("books");

            return string.Join(" ", classes.Distinct());
        }
        public async Task<IActionResult> Book_media(
     string keywords = "",
     int? catalogId = null,
     int? categoryId = null,
     string orderby = "name",
     string lang = "",
     string year = "",
     int page = 1)
        {
            var query = _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .Include(k => k.KitobNusxalari)
                .AsQueryable();

            // 1. Kalit so‘z qidiruvi
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.Trim();
                query = query.Where(k => k.Nomi.Contains(keywords) ||
                    k.KitobMualliflari.Any(km => km.Muallif.Ism.Contains(keywords) || km.Muallif.Familiya.Contains(keywords)) ||
                    k.KitobKategoriyalari.Any(kk => kk.Kategoriya.Nomi.Contains(keywords)));
            }

            // 2. Katalog filtri
            if (catalogId.HasValue && catalogId.Value >= 1 && catalogId.Value <= 5)
            {
                MediaType mediaType = catalogId.Value switch
                {
                    1 => MediaType.Kitob,
                    2 => MediaType.ElektronKitob,
                    3 => MediaType.AudioKitob,
                    4 => MediaType.Jurnal,
                    5 => MediaType.Video,
                    _ => MediaType.Kitob
                };
                query = query.Where(k => k.MediaTuri == mediaType);
            }

            // 3. Kategoriya filtri
            if (categoryId.HasValue)
            {
                query = query.Where(k => k.KitobKategoriyalari.Any(kk => kk.KategoriyaId == categoryId.Value));
            }

            // 4. Til filtri
            if (!string.IsNullOrEmpty(lang))
            {
                query = query.Where(k => k.Til == lang);
            }

            // 5. Saralash (orderby) – IOrderedQueryable olish
            IOrderedQueryable<Kitob> orderedQuery;
            switch (orderby)
            {
                case "popular":
                    orderedQuery = query.OrderByDescending(k => k.KorishlarSoni);
                    break;
                case "rating":
                    orderedQuery = query.OrderByDescending(k => k.OrtachaBaho);
                    break;
                case "new":
                    orderedQuery = query.OrderByDescending(k => k.NashrYili);
                    break;
                case "price":
                    orderedQuery = query.OrderBy(k => k.Narxi);
                    break;
                default:
                    orderedQuery = query.OrderBy(k => k.Nomi);
                    break;
            }

            // 6. Yil bo‘yicha qo‘shimcha saralash (agar asosiy saralash "new" bo‘lmasa)
            if (!string.IsNullOrEmpty(year) && orderby != "new")
            {
                if (year == "asc")
                    orderedQuery = orderedQuery.ThenBy(k => k.NashrYili);
                else if (year == "desc")
                    orderedQuery = orderedQuery.ThenByDescending(k => k.NashrYili);
            }

            // 7. Sahifalash
            int totalItems = await orderedQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            page = page < 1 ? 1 : (page > totalPages && totalPages > 0 ? totalPages : page);
            var items = await orderedQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Keywords = keywords;
            ViewBag.SelectedCatalog = catalogId;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.OrderBy = orderby;
            ViewBag.Lang = lang;
            ViewBag.Year = year;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = await _context.Kategoriyalar.ToListAsync();

            return View(items);
        }
        public async Task<IActionResult> Book_media_detail(int id)
        {
            var book = await _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .Include(k => k.KitobNusxalari)
                .Include(k => k.Reytinglar)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (book == null) return NotFound();

            // Yaqinda qo‘shilgan kitoblar (sidebar uchun)
            ViewBag.RecentBooks = await _context.Kitoblar
                .OrderByDescending(k => k.QoshilganSana)
                .Take(5)
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .ToListAsync();

            // Kategoriyalar (sidebar uchun)
            ViewBag.Categories = await _context.Kategoriyalar
                .Where(k => k.OtaKategoriyaId == null)
                .Include(k => k.IchkiKategoriyalar)
                .ToListAsync();

            return View(book);
        }
        public IActionResult News()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        // GET: /Home/Contact
        [Authorize]
        public IActionResult Contact()
        {
            var model = new ContactMessage();
            // Agar foydalanuvchi ma’lumotlarini avtomatik to‘ldirishni xohlasangiz:
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                model.Ism = user.Ism;
                model.Familiya = user.Familiya;
                model.Email = user.Email ?? string.Empty;
                model.Telefon = user.Telefon;
            }
            return View(model);
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["Error"] = "Iltimos, barcha maydonlarni to‘g‘ri to‘ldiring.";
        //        return View(model);
        //    }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Challenge();
            }

            var message = new ContactMessage
            {
                UserId = userId ?? string.Empty,
                Ism = model.Ism ?? user.Ism ?? string.Empty,
                Familiya = model.Familiya ?? user.Familiya ?? string.Empty,
                Email = model.Email ?? user.Email ?? string.Empty,
                Telefon = model.Telefon ?? user.Telefon,
                Xabar = model.Xabar,
                YuborilganSana = DateTime.Now,
                Oqilgan = false
            };
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xabaringiz muvaffaqiyatli yuborildi. Tez orada javob olasiz.";
            return RedirectToAction(nameof(Contact));
        }
    
    }
}
