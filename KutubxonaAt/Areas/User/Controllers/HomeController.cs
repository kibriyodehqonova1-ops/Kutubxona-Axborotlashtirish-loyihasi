using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string keywords, int? catalogId, int? categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kutubxona umumiy statistikasi
            var eBookCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.ElektronKitob);
            var audioBookCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.AudioKitob);
            var journalCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.Jurnal);
            var videoCount = await _context.Kitoblar.CountAsync(k => k.MediaTuri == MediaType.Video);

            // Foydalanuvchi shaxsiy statistikasi
            ViewBag.ActiveLoansCount = await _context.Beruvlar
                .CountAsync(b => b.FoydalanuvchiId == userId && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan"));
            ViewBag.ActiveReservationsCount = await _context.Bronlar
                .CountAsync(b => b.FoydalanuvchiId == userId && b.Holati == "Faol");
            ViewBag.PendingFinesCount = await _context.Jarimalar
                .CountAsync(j => j.FoydalanuvchiId == userId && j.Holati == "Kutilmoqda");
            ViewBag.PendingFinesTotal = await _context.Jarimalar
                .Where(j => j.FoydalanuvchiId == userId && j.Holati == "Kutilmoqda")
                .SumAsync(j => (decimal?)j.Miqdor) ?? 0;
            ViewBag.OverdueLoansCount = await _context.Beruvlar
                .CountAsync(b => b.FoydalanuvchiId == userId
                    && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan")
                    && b.QaytarishMuddati < DateTime.Now);

            // So’nggi 6 kitob
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
                LeadText = "Bilim va ma’rifat maskani",
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
    }
}