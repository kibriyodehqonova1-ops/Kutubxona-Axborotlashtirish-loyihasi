using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Services
{
    public class BildirishnomaService : IBildirishnomaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BildirishnomaService> _logger;

        public BildirishnomaService(ApplicationDbContext context, ILogger<BildirishnomaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task YuborAsync(string foydalanuvchiId, string sarlavha, string matn,
            string tur = "Umumiy", string? havola = null)
        {
            var bildirishnoma = new Bildirishnoma
            {
                FoydalanuvchiId = foydalanuvchiId,
                Sarlavha = sarlavha,
                Matn = matn,
                Tur = tur,
                Havola = havola,
                YuborilganSana = DateTime.Now,
                Oqilgan = false
            };
            _context.Bildirishnomalar.Add(bildirishnoma);
            await _context.SaveChangesAsync();
        }

        public async Task KopYuborAsync(IEnumerable<string> foydalanuvchiIdlar, string sarlavha,
            string matn, string tur = "Umumiy", string? havola = null)
        {
            var bildirishnomalar = foydalanuvchiIdlar.Select(id => new Bildirishnoma
            {
                FoydalanuvchiId = id,
                Sarlavha = sarlavha,
                Matn = matn,
                Tur = tur,
                Havola = havola,
                YuborilganSana = DateTime.Now,
                Oqilgan = false
            }).ToList();

            _context.Bildirishnomalar.AddRange(bildirishnomalar);
            await _context.SaveChangesAsync();
        }

        /// <summary>Abonement tugashiga 3 kun va tugaganiga eslatma yuborish (background job uchun)</summary>
        public async Task AbonementEslatmaYuborAsync()
        {
            var bugun = DateTime.Today;
            var uchKunKeyin = bugun.AddDays(3);

            // Tugashiga 3 kun qolganlar
            var yaqinTugaydiganlar = await _context.Abonementlar
                .Where(a => a.Holati == "Faol"
                    && !a.Eslatma3KunYuborildi
                    && a.TugashSana.Date == uchKunKeyin)
                .Include(a => a.AbonementTuri)
                .ToListAsync();

            foreach (var ab in yaqinTugaydiganlar)
            {
                await YuborAsync(
                    ab.FoydalanuvchiId,
                    "⚠️ Abonementingiz 3 kunda tugaydi",
                    $"Hurmatli foydalanuvchi, '{ab.AbonementTuri?.Nomi}' abonementingiz " +
                    $"{ab.TugashSana:dd.MM.yyyy} sanada tugaydi. Uzaytirish uchun profilingizga o'ting.",
                    "Abonement",
                    "/User/Subscription/Index"
                );
                ab.Eslatma3KunYuborildi = true;
            }

            // Muddati o'tganlar
            var tugaganlar = await _context.Abonementlar
                .Where(a => a.Holati == "Faol"
                    && !a.EslatmaTugaganYuborildi
                    && a.TugashSana.Date < bugun)
                .ToListAsync();

            foreach (var ab in tugaganlar)
            {
                ab.Holati = "Tugagan";
                await YuborAsync(
                    ab.FoydalanuvchiId,
                    "❌ Abonementingiz tugadi",
                    "Kutubxona abonementingiz muddati tugadi. Xizmatlardan to'liq foydalanish uchun yangi abonement rasmiylashtiring.",
                    "Abonement",
                    "/User/Subscription/Index"
                );
                ab.EslatmaTugaganYuborildi = true;
            }

            if (yaqinTugaydiganlar.Any() || tugaganlar.Any())
                await _context.SaveChangesAsync();
        }

        /// <summary>Kitob qaytarish muddati eslatmalari: 3, 2, 1 kun qolganda va o'tganda</summary>
        public async Task QaytarishEslatmaYuborAsync()
        {
            var bugun = DateTime.Today;

            var faolBeruvlar = await _context.Beruvlar
                .Where(b => b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan")
                .Include(b => b.KitobNusxa)
                    .ThenInclude(kn => kn!.Kitob)
                .ToListAsync();

            foreach (var beruv in faolBeruvlar)
            {
                var qolganKunlar = (beruv.QaytarishMuddati.Date - bugun).Days;
                var kitobNomi = beruv.KitobNusxa?.Kitob?.Nomi ?? "Kitob";

                if (qolganKunlar == 3 || qolganKunlar == 2 || qolganKunlar == 1)
                {
                    // Allaqachon bugungi eslatma yuborilganmi tekshirib ko'rish kerak
                    // Oddiy holatda: har safar yuboramiz (production'da flag qo'shish kerak)
                    await YuborAsync(
                        beruv.FoydalanuvchiId,
                        $"📅 Kitob qaytarish muddati: {qolganKunlar} kun qoldi",
                        $"'{kitobNomi}' kitobini {beruv.QaytarishMuddati:dd.MM.yyyy} sanasiga qadar qaytaring. " +
                        $"Kechiktirilsa, kuniga 1 000 so'm jarima hisoblanadi.",
                        "Muddat",
                        "/User/Loan/Index"
                    );
                }
                else if (qolganKunlar < 0)
                {
                    beruv.Holati = "MuddatiO'tgan";
                    await YuborAsync(
                        beruv.FoydalanuvchiId,
                        $"🚨 Kitob qaytarish muddati o'tdi!",
                        $"'{kitobNomi}' kitobini qaytarish muddati {Math.Abs(qolganKunlar)} kun oldin o'tdi. " +
                        $"Iltimos, zudlik bilan qaytaring. Hozirgi jarima: {Math.Abs(qolganKunlar) * LibraryRules.DailyFineAmount:N0} so'm.",
                        "Jarima",
                        "/User/Loan/Index"
                    );
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
