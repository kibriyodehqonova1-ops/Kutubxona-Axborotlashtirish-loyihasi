using KutubxonaAt.Data;
using KutubxonaAt.Models;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Services
{
    /// <summary>
    /// Admin tasdiqlashisiz avtomatik kitob berish xizmati.
    /// Bron qilingan kitob avtomatik tekshiriladi va Beruv yozuvi yaratiladi.
    /// </summary>
    public class AutoBerishService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBildirishnomaService _bildirishnoma;
        private readonly ILogger<AutoBerishService> _logger;

        private const int KunlikJarima = LibraryRules.DailyFineAmount; // so'm
        private const int MaksUzaytirish = 3;
        private const int UzaytirishKun = 15;
        private const int BeruvMuddatKun = 30;

        public AutoBerishService(ApplicationDbContext context,
            IBildirishnomaService bildirishnoma,
            ILogger<AutoBerishService> logger)
        {
            _context = context;
            _bildirishnoma = bildirishnoma;
            _logger = logger;
        }

        public class AutoBerishNatija
        {
            public bool Muvaffaqiyatli { get; set; }
            public string Xabar { get; set; } = string.Empty;
            public Beruv? YaratilganBeruv { get; set; }
            public string? RadEtishSababi { get; set; }
        }

        /// <summary>
        /// Brondan keyin avtomatik berishni bajaradi.
        /// Foydalanuvchi shartlarini tekshiradi va nusxa ajratadi.
        /// </summary>
        public async Task<AutoBerishNatija> AvtomatikBerishAsync(int bronId)
        {
            var bron = await _context.Bronlar
                .Include(b => b.Foydalanuvchi)
                .Include(b => b.Kitob)
                    .ThenInclude(k => k!.KitobNusxalari)
                .FirstOrDefaultAsync(b => b.Id == bronId);

            if (bron == null)
                return Rad("Bron topilmadi.");

            if (bron.Holati != "Faol")
                return Rad("Bron faol emas.");

            var foydalanuvchi = bron.Foydalanuvchi;
            if (foydalanuvchi == null)
                return Rad("Foydalanuvchi topilmadi.");

            // 1. Foydalanuvchi faolligini tekshirish
            if (!foydalanuvchi.Faol)
                return await RadEtishBildiruv(bron, "Hisobingiz faol emas. Kutubxona xodimiga murojaat qiling.");

            // 2. To'lanmagan jarima tekshirish
            var jarimaBor = await _context.Jarimalar
                .AnyAsync(j => j.FoydalanuvchiId == foydalanuvchi.Id && j.Holati == "Kutilmoqda");
            if (jarimaBor)
                return await RadEtishBildiruv(bron, "To'lanmagan jarimangiz mavjud. Jarimani to'lagandan so'ng kitob olishingiz mumkin.");

            // 3. Faol beruvlar soni tekshirish (maks 5 ta)
            var faolBeruvlarSoni = await _context.Beruvlar
                .CountAsync(b => b.FoydalanuvchiId == foydalanuvchi.Id
                    && (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan"));
            if (faolBeruvlarSoni >= 5)
                return await RadEtishBildiruv(bron, "Bir vaqtda 5 tadan ko'p kitob olish mumkin emas. Avval biror kitobni qaytaring.");

            // 4. Mavjud nusxani topish
            var mavjudNusxa = bron.KitobNusxaId.HasValue
                ? await _context.KitobNusxalari.FirstOrDefaultAsync(n =>
                    n.Id == bron.KitobNusxaId && n.Holati == "Mavjud")
                : await _context.KitobNusxalari.FirstOrDefaultAsync(n =>
                    n.KitobId == bron.KitobId
                    && n.Holati == "Mavjud"
                    && !n.OquvZalidanChiqmaydi);

            if (mavjudNusxa == null)
            {
                // Nusxa yo'q — kutish ro'yxatiga avtomatik qo'shish
                await KutishRoyxatiQoshAsync(bron.FoydalanuvchiId, bron.KitobId);
                var eng_erta = await EngErtaQaytishSanasiniOlAsync(bron.KitobId);
                var eslatma = eng_erta.HasValue
                    ? $" Taxminiy mavjud bo'lish sanasi: {eng_erta.Value:dd.MM.yyyy}."
                    : string.Empty;
                return Rad($"Hozirda barcha nusxalar berilgan.{eslatma} Siz kutish ro'yxatiga qo'shildingiz.");
            }

            // 5. Beruv yaratish
            var beruv = new Beruv
            {
                FoydalanuvchiId = foydalanuvchi.Id,
                KitobNusxaId = mavjudNusxa.Id,
                BeruvRaqami = Guid.NewGuid().ToString()[..8].ToUpper(),
                BerilganSana = DateTime.Now,
                QaytarishMuddati = DateTime.Now.AddDays(BeruvMuddatKun),
                Holati = "Berilgan",
                BerganXodim = "Avtomatik tizim"
            };
            _context.Beruvlar.Add(beruv);

            // 6. Nusxa holatini yangilash
            mavjudNusxa.Holati = "Berilgan";

            // 7. Kitob mavjud nusxalar sonini kamaytirish
            if (bron.Kitob != null)
                bron.Kitob.MavjudNusxalar = Math.Max(0, bron.Kitob.MavjudNusxalar - 1);

            // 8. Bronni bajarilgan deb belgilash
            bron.Holati = "Bajarilgan";
            bron.KitobNusxaId = mavjudNusxa.Id;

            await _context.SaveChangesAsync();

            // 9. Muvaffaqiyat bildiruvini yuborish
            await _bildirishnoma.YuborAsync(
                foydalanuvchi.Id,
                "✅ Kitob tayyor!",
                $"'{bron.Kitob?.Nomi}' kitobini kutubxonadan olishingiz mumkin. " +
                $"Qaytarish muddati: {beruv.QaytarishMuddati:dd.MM.yyyy}. " +
                $"Beruv raqami: #{beruv.BeruvRaqami}",
                "Beruv",
                "/User/Loan/Index"
            );

            _logger.LogInformation("Avtomatik beruv yaratildi: Foydalanuvchi={UserId}, Kitob={KitobId}, Beruv={BeruvId}",
                foydalanuvchi.Id, bron.KitobId, beruv.Id);

            return new AutoBerishNatija
            {
                Muvaffaqiyatli = true,
                Xabar = $"'{bron.Kitob?.Nomi}' kitobini kutubxonadan olishingiz mumkin. Beruv raqami: #{beruv.BeruvRaqami}",
                YaratilganBeruv = beruv
            };
        }

        /// <summary>Kitob qaytarilganda kutish ro'yxatidagi birinchi kishiga bildiruv yuborish</summary>
        public async Task KutishRoyxatiUchunBildirishAsync(int kitobId)
        {
            var birinchi = await _context.KutishRoyxati
                .Where(k => k.KitobId == kitobId && k.Holati == "Kutmoqda")
                .OrderBy(k => k.Tartib).ThenBy(k => k.QoshilganSana)
                .Include(k => k.Kitob)
                .FirstOrDefaultAsync();

            if (birinchi == null) return;

            birinchi.Holati = "Bildirildi";
            birinchi.BildirilganSana = DateTime.Now;
            await _context.SaveChangesAsync();

            await _bildirishnoma.YuborAsync(
                birinchi.FoydalanuvchiId,
                "📚 Kitob mavjud bo'ldi!",
                $"Siz kutayotgan '{birinchi.Kitob?.Nomi}' kitob endi mavjud. " +
                $"Tezroq bron qiling — boshqa o'quvchilar ham kutmoqda!",
                "KutishRoyxati",
                $"/User/Reservation/Create/{kitobId}"
            );
        }

        /// <summary>Muddat uzaytirish (foydalanuvchi tomonidan)</summary>
        public async Task<AutoBerishNatija> UzaytirishAsync(int beruvId, string foydalanuvchiId)
        {
            var beruv = await _context.Beruvlar
                .Include(b => b.KitobNusxa)
                    .ThenInclude(kn => kn!.Kitob)
                .FirstOrDefaultAsync(b => b.Id == beruvId && b.FoydalanuvchiId == foydalanuvchiId);

            if (beruv == null) return Rad("Beruv topilmadi.");

            if (beruv.Holati != "Berilgan" && beruv.Holati != "Uzaytirilgan")
                return Rad("Bu beruv uzaytirib bo'lmaydi.");

            if (beruv.UzaytirishlarSoni >= MaksUzaytirish)
                return Rad($"Uzaytirish imkoniyati tugagan ({MaksUzaytirish} marta).");

            if (beruv.QaytarishMuddati < DateTime.Now)
                return Rad("Muddati o'tgan beruvni uzaytirish mumkin emas. Avval jarimani to'lang.");

            var jarimaBor = await _context.Jarimalar
                .AnyAsync(j => j.FoydalanuvchiId == foydalanuvchiId && j.Holati == "Kutilmoqda");
            if (jarimaBor)
                return Rad("To'lanmagan jarima mavjud. Avval jarimani to'lang.");

            beruv.QaytarishMuddati = beruv.QaytarishMuddati.AddDays(UzaytirishKun);
            beruv.UzaytirishlarSoni++;
            beruv.Holati = "Uzaytirilgan";
            beruv.UzaytirilganMuddat = DateTime.Now;

            await _context.SaveChangesAsync();

            var kitobNomi = beruv.KitobNusxa?.Kitob?.Nomi ?? "Kitob";
            await _bildirishnoma.YuborAsync(
                foydalanuvchiId,
                "✅ Muddat uzaytirildi",
                $"'{kitobNomi}' kitobining qaytarish muddati {UzaytirishKun} kunga uzaytirildi. " +
                $"Yangi muddat: {beruv.QaytarishMuddati:dd.MM.yyyy}. " +
                $"Qolgan uzaytirish huquqi: {MaksUzaytirish - beruv.UzaytirishlarSoni} ta.",
                "Beruv"
            );

            return new AutoBerishNatija
            {
                Muvaffaqiyatli = true,
                Xabar = $"Muddat {UzaytirishKun} kunga uzaytirildi. Yangi muddat: {beruv.QaytarishMuddati:dd.MM.yyyy}",
                YaratilganBeruv = beruv
            };
        }

        // ======================== Helpers ========================

        private async Task<DateTime?> EngErtaQaytishSanasiniOlAsync(int kitobId)
        {
            return await _context.Beruvlar
                .Where(b => (b.Holati == "Berilgan" || b.Holati == "Uzaytirilgan")
                    && b.KitobNusxa != null && b.KitobNusxa.KitobId == kitobId)
                .MinAsync(b => (DateTime?)b.QaytarishMuddati);
        }

        private async Task KutishRoyxatiQoshAsync(string foydalanuvchiId, int kitobId)
        {
            var bormi = await _context.KutishRoyxati
                .AnyAsync(k => k.FoydalanuvchiId == foydalanuvchiId
                    && k.KitobId == kitobId
                    && k.Holati == "Kutmoqda");
            if (bormi) return;

            var tartib = await _context.KutishRoyxati
                .Where(k => k.KitobId == kitobId && k.Holati == "Kutmoqda")
                .CountAsync() + 1;

            _context.KutishRoyxati.Add(new KutishRoyxati
            {
                FoydalanuvchiId = foydalanuvchiId,
                KitobId = kitobId,
                QoshilganSana = DateTime.Now,
                Holati = "Kutmoqda",
                Tartib = tartib
            });
            await _context.SaveChangesAsync();
        }

        private static AutoBerishNatija Rad(string sabab) =>
            new() { Muvaffaqiyatli = false, Xabar = sabab, RadEtishSababi = sabab };

        private async Task<AutoBerishNatija> RadEtishBildiruv(Bron bron, string sabab)
        {
            await _bildirishnoma.YuborAsync(
                bron.FoydalanuvchiId,
                "❌ Kitob berish rad etildi",
                $"'{bron.Kitob?.Nomi}' uchun bron rad etildi. Sabab: {sabab}",
                "Beruv"
            );
            return Rad(sabab);
        }
    }
}
