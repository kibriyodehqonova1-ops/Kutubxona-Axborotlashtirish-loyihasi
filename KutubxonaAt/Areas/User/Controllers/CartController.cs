using KutubxonaAt.Data;
using KutubxonaAt.Extensions;
using KutubxonaAt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User,Admin,Librarian")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "ShoppingCart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cart ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }

        // Savatga qo‘shish (kitob detalidan yoki katalogdan)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int kitobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var kitob = await _context.Kitoblar
                .Include(k => k.KitobMualliflari).ThenInclude(km => km.Muallif)
                .Include(k => k.KitobNusxalari)
                .FirstOrDefaultAsync(k => k.Id == kitobId);
            if (kitob == null) return NotFound();

            bool isDigital = kitob.MediaTuri == Kitob.MediaType.ElektronKitob
                          || kitob.MediaTuri == Kitob.MediaType.AudioKitob
                          || kitob.MediaTuri == Kitob.MediaType.Video;

            KitobNusxa? nusxa;
            if (isDigital)
            {
                // Digital materiallar uchun narxi > 0 bo’lsa sotib olish mumkin (∞ nusxa)
                if (!kitob.Narxi.HasValue || kitob.Narxi <= 0)
                {
                    TempData["Error"] = "Bu material bepul — sotib olish shart emas.";
                    return RedirectToAction("Details", "Book", new { id = kitobId });
                }
                // Mavjud DIG- nusxani olamiz (yoki birinchisini)
                nusxa = kitob.KitobNusxalari.FirstOrDefault(kn => kn.Holati == "Mavjud")
                     ?? kitob.KitobNusxalari.FirstOrDefault();
                if (nusxa == null)
                {
                    TempData["Error"] = "Material topilmadi.";
                    return RedirectToAction("Details", "Book", new { id = kitobId });
                }
            }
            else
            {
                // Jismoniy kitob/jurnal: sotuvga qo’yilgan va mavjud nusxa kerak
                nusxa = kitob.KitobNusxalari.FirstOrDefault(kn => kn.Holati == "Mavjud" && kn.SotuvgaQoyilgan);
                if (nusxa == null)
                {
                    TempData["Error"] = "Bu kitob hozirda sotuvda mavjud emas.";
                    return RedirectToAction("Details", "Book", new { id = kitobId });
                }
            }

            var mualliflar = string.Join(", ", kitob.KitobMualliflari.Select(km => $"{km.Muallif.Familiya} {km.Muallif.Ism}"));

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.KitobId == kitobId);
            if (existing == null)
            {
                cart.Add(new CartItem
                {
                    KitobId = kitobId,
                    Nomi = kitob.Nomi,
                    Rasm = kitob.Rasm,
                    Narxi = kitob.Narxi ?? 0,
                    KitobNusxaId = nusxa.Id,
                    Mualliflar = mualliflar,
                    ISBN = kitob.ISBN ?? "Mavjud emas"
                });
                SaveCart(cart);
                TempData["Success"] = "Kitob savatga qo‘shildi.";
            }
            else
            {
                TempData["Info"] = "Kitob allaqachon savatda.";
            }
            return RedirectToAction("Index");
        }

        // Savatdan o‘chirish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int kitobId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.KitobId == kitobId);
            SaveCart(cart);
            TempData["Success"] = "Kitob savatdan olib tashlandi.";
            return RedirectToAction("Index");
        }

        // Savat sahifasi
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(c => c.Narxi);
            return View(cart);
        }

        // Checkout sahifasi (to‘lov va manzil formasi)
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }
            ViewBag.Total = cart.Sum(c => c.Narxi);
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string tolovUsuli, string manzil, string izoh)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }

            decimal jami = cart.Sum(c => c.Narxi);
            var order = new Order
            {
                FoydalanuvchiId = userId ?? string.Empty,
                Sana = DateTime.Now,
                JamiSumma = jami,
                Holati = "Kutilmoqda",
                TolovUsuli = tolovUsuli,
                Manzil = manzil,
                Izoh = izoh
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var kitob = await _context.Kitoblar.FindAsync(item.KitobId);
                if (kitob == null) continue;

                bool itemIsDigital = kitob.MediaTuri == MediaType.ElektronKitob
                                  || kitob.MediaTuri == MediaType.AudioKitob
                                  || kitob.MediaTuri == MediaType.Video;

                var nusxa = await _context.KitobNusxalari.FindAsync(item.KitobNusxaId);

                if (itemIsDigital)
                {
                    // Digital: nusxa holatini o'zgartirmaymiz — cheksiz sotilishi mumkin
                    if (nusxa == null) continue;
                }
                else
                {
                    // Jismoniy: nusxa sotuvda va mavjud bo'lishi shart
                    if (nusxa == null || nusxa.Holati != "Mavjud" || !nusxa.SotuvgaQoyilgan)
                        continue;

                    // Nusxani sotilgan deb belgilaymiz
                    nusxa.Holati = "Sotilgan";
                    kitob.MavjudNusxalar = Math.Max(0, kitob.MavjudNusxalar - 1);
                }

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId      = order.Id,
                    KitobId      = item.KitobId,
                    KitobNusxaId = item.KitobNusxaId,
                    Narxi        = item.Narxi
                });
            }
            await _context.SaveChangesAsync();

            SaveCart(new List<CartItem>());

            TempData["Success"] = $"Buyurtma #{order.Id} qabul qilindi. Jami: {jami:N0} so‘m. To‘lov usuli: {tolovUsuli}.";

            if (tolovUsuli == "Click" || tolovUsuli == "Payme" || tolovUsuli == "Plastik karta")
            {
                return RedirectToAction("PaymentRedirect", new { orderId = order.Id, amount = jami });
            }

            return RedirectToAction("Index", "Purchase");
        }
        // To‘lovga yo‘naltirish (test)
        public IActionResult PaymentRedirect(int orderId, decimal amount)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            return View();
        }
    }
}