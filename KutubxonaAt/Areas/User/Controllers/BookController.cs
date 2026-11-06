using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 9;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
         string keywords = "",
         int? catalogId = null,
         int? categoryId = null,
         string sortBy = "name_asc",
         bool onSale = false,
         int page = 1)
        {
            var query = _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .Include(k => k.KitobNusxalari)  // Nusxalarni ham yuklaymiz
                .AsQueryable();

            // Qidiruv
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.Trim();
                query = query.Where(k => k.Nomi.Contains(keywords) ||
                    k.KitobMualliflari.Any(km => km.Muallif.Ism.Contains(keywords) || km.Muallif.Familiya.Contains(keywords)) ||
                    k.KitobKategoriyalari.Any(kk => kk.Kategoriya.Nomi.Contains(keywords)));
            }

            // Katalog (MediaType) filtri
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

            // Kategoriya filtri
            if (categoryId.HasValue)
            {
                query = query.Where(k => k.KitobKategoriyalari.Any(kk => kk.KategoriyaId == categoryId.Value));
            }

            // ** Yangi: faqat sotuvdagi kitoblar **
            if (onSale)
            {
                query = query.Where(k => k.KitobNusxalari.Any(kn => kn.SotuvgaQoyilgan == true && kn.Holati == "Mavjud"));
            }

            // Saralash
            query = sortBy switch
            {
                "name_asc" => query.OrderBy(k => k.Nomi),
                "name_desc" => query.OrderByDescending(k => k.Nomi),
                "popular" => query.OrderByDescending(k => k.KorishlarSoni),
                "rating" => query.OrderByDescending(k => k.OrtachaBaho),
                "new" => query.OrderByDescending(k => k.NashrYili),
                "price_asc" => query.OrderBy(k => k.Narxi),
                "price_desc" => query.OrderByDescending(k => k.Narxi),
                _ => query.OrderBy(k => k.Nomi)
            };

            // Pagination
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));
            var items = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Keywords = keywords;
            ViewBag.SelectedCatalog = catalogId;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.OnSale = onSale;               // Yangi
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = await _context.Kategoriyalar.ToListAsync();

            return View(items);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobKategoriyalari).ThenInclude(kk => kk.Kategoriya)
                .Include(k => k.KitobNusxalari)
                .FirstOrDefaultAsync(k => k.Id == id);
            if (book == null) return NotFound();

            bool isDigital = book.MediaTuri == MediaType.ElektronKitob
                          || book.MediaTuri == MediaType.AudioKitob
                          || book.MediaTuri == MediaType.Video;

            // Bron — faqat jismoniy materiallar (Kitob, Jurnal)
            int availableForReservation = isDigital
                ? 0
                : book.KitobNusxalari.Count(kn => kn.Holati == "Mavjud");

            // Sotuv — digital uchun narxi > 0 bo’lsa har doim mavjud (∞ nusxa)
            //         jismoniy uchun SotuvgaQoyilgan=true va Holati="Mavjud" nusxalar soni
            int availableForPurchase = isDigital
                ? (book.Narxi.HasValue && book.Narxi > 0 ? 1 : 0)
                : book.KitobNusxalari.Count(kn => kn.Holati == "Mavjud" && kn.SotuvgaQoyilgan);

            // Eng erta qaytariladigan nusxa (agar kitob olingan bo‘lsa)
            DateTime? earliestReturnDate = null;
            var borrowedCopy = book.KitobNusxalari.FirstOrDefault(kn => kn.Holati == "Berilgan");
            if (borrowedCopy != null)
            {
                var beruv = await _context.Beruvlar
                    .Where(b => b.KitobNusxaId == borrowedCopy.Id && b.Holati == "Berilgan")
                    .OrderBy(b => b.QaytarishMuddati)
                    .FirstOrDefaultAsync();
                if (beruv != null)
                    earliestReturnDate = beruv.QaytarishMuddati;
            }

            // Digital materiallarni foydalanuvchi allaqachon xarid qilganmi?
            bool alreadyPurchased = false;
            if (isDigital && User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                alreadyPurchased = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .AnyAsync(oi => oi.KitobId == id
                                 && oi.Order!.FoydalanuvchiId == userId
                                 && oi.Order.Holati == "To'langan");
            }

            ViewBag.AvailableCopies    = availableForReservation;
            ViewBag.ForSaleCopies      = availableForPurchase;
            ViewBag.EarliestReturnDate = earliestReturnDate;
            ViewBag.IsDigital          = isDigital;
            ViewBag.AlreadyPurchased   = alreadyPurchased;

            return View(book);
        }
    }
}