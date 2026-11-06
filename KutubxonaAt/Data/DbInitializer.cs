using KutubxonaAt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KutubxonaAt.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                string[] roles = { "Admin", "Librarian", "User" };
                foreach (var role in roles)
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));

                var users = await SeedUsersAsync(userManager, context);
                await SeedKategoriyalarAsync(context);
                await SeedMualliflarAsync(context);
                await SeedKitoblarAsync(context);
                await SeedAbonementTurlariAsync(context);
                await SeedAbonementlarAsync(context, users);
                await SeedBronlarVaBeruvlarAsync(context, users);
                await SeedReytinglarAsync(context, users);
                await SeedBildirishnomalarAsync(context, users);
                await SeedTestMateriallarAsync(context);    // har turdan >= 4 ta bo'lishini ta'minlaydi
                await SeedSotuvgaQoyilganAsync(context);   // har doim ishlaydi

                logger.LogInformation("✅ Seed muvaffaqiyatli tugadi.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Seed xatosi.");
            }
        }

        // ==================== FOYDALANUVCHILAR ====================
        private static async Task<List<ApplicationUser>> SeedUsersAsync(
            UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            if (await context.Users.CountAsync() > 1)
                return await context.Users.Where(u => u.Turi != "Admin").Take(10).ToListAsync();

            var users = new List<(ApplicationUser user, string password, string role)>
            {
                (new ApplicationUser { UserName = "librarian@kutubxona.uz", Email = "librarian@kutubxona.uz",
                    Ism = "Nodira", Familiya = "Yusupova", Turi = "Librarian",
                    Telefon = "+998901234567", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-6),
                    AmalMuddati = DateTime.Now.AddYears(1), EmailConfirmed = true,
                    Manzil = "Toshkent, Yunusobod tumani" }, "Librarian@2025!", "Librarian"),

                (new ApplicationUser { UserName = "ali.karimov@student.uz", Email = "ali.karimov@student.uz",
                    Ism = "Ali", Familiya = "Karimov", Turi = "Talaba",
                    Fakultet = "Kompyuter muhandisligi", Guruh = "KM-21", TalabaId = "21KM0045",
                    Telefon = "+998901112233", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-4),
                    AmalMuddati = DateTime.Now.AddMonths(8), EmailConfirmed = true,
                    Manzil = "Toshkent, Chilonzor, 15-mavze" }, "User@2025!", "User"),

                (new ApplicationUser { UserName = "zulfiya.nazarova@student.uz", Email = "zulfiya.nazarova@student.uz",
                    Ism = "Zulfiya", Familiya = "Nazarova", Turi = "Talaba",
                    Fakultet = "Iqtisodiyot", Guruh = "IQ-22", TalabaId = "22IQ0078",
                    Telefon = "+998912223344", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-3),
                    AmalMuddati = DateTime.Now.AddMonths(9), EmailConfirmed = true,
                    Manzil = "Toshkent, Mirzo Ulug'bek" }, "User@2025!", "User"),

                (new ApplicationUser { UserName = "bobur.toshmatov@student.uz", Email = "bobur.toshmatov@student.uz",
                    Ism = "Bobur", Familiya = "Toshmatov", Turi = "Talaba",
                    Fakultet = "Matematika", Guruh = "MT-23", TalabaId = "23MT0112",
                    Telefon = "+998933334455", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-2),
                    AmalMuddati = DateTime.Now.AddMonths(10), EmailConfirmed = true,
                    Manzil = "Toshkent, Shayxontohur" }, "User@2025!", "User"),

                (new ApplicationUser { UserName = "malika.xasanova@teacher.uz", Email = "malika.xasanova@teacher.uz",
                    Ism = "Malika", Familiya = "Xasanova", Turi = "O'qituvchi",
                    Fakultet = "Filologiya", Telefon = "+998944445566",
                    Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-8),
                    AmalMuddati = DateTime.Now.AddYears(1), EmailConfirmed = true,
                    Manzil = "Toshkent, Uchtepa" }, "User@2025!", "User"),

                (new ApplicationUser { UserName = "jasur.rahimov@student.uz", Email = "jasur.rahimov@student.uz",
                    Ism = "Jasur", Familiya = "Rahimov", Turi = "Talaba",
                    Fakultet = "Fizika", Guruh = "FZ-21", TalabaId = "21FZ0089",
                    Telefon = "+998955556677", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-5),
                    AmalMuddati = DateTime.Now.AddMonths(7), EmailConfirmed = true,
                    Manzil = "Toshkent, Bektemir" }, "User@2025!", "User"),

                (new ApplicationUser { UserName = "dilnoza.ergasheva@student.uz", Email = "dilnoza.ergasheva@student.uz",
                    Ism = "Dilnoza", Familiya = "Ergasheva", Turi = "Talaba",
                    Fakultet = "Tarix", Guruh = "TX-22", TalabaId = "22TX0034",
                    Telefon = "+998966667788", Faol = true, RoyxatSanasi = DateTime.Now.AddMonths(-3),
                    AmalMuddati = DateTime.Now.AddMonths(9), EmailConfirmed = true,
                    Manzil = "Toshkent, Olmazor" }, "User@2025!", "User"),
            };

            var result = new List<ApplicationUser>();
            foreach (var (user, password, role) in users)
            {
                var existing = await userManager.FindByEmailAsync(user.Email!);
                if (existing == null)
                {
                    var r = await userManager.CreateAsync(user, password);
                    if (r.Succeeded) { await userManager.AddToRoleAsync(user, role); result.Add(user); }
                }
                else result.Add(existing);
            }
            return result;
        }

        // ==================== KATEGORIYALAR ====================
        private static async Task SeedKategoriyalarAsync(ApplicationDbContext ctx)
        {
            if (await ctx.Kategoriyalar.AnyAsync()) return;
            ctx.Kategoriyalar.AddRange(
                new Kategoriya { Nomi = "Adabiyot", Tavsif = "Badiiy adabiyot asarlari" },
                new Kategoriya { Nomi = "Fan va texnologiya", Tavsif = "Ilmiy va texnik kitoblar" },
                new Kategoriya { Nomi = "Tarix", Tavsif = "Tarix va siyosat" },
                new Kategoriya { Nomi = "Falsafa va psixologiya", Tavsif = "Falsafa va psixologiya" },
                new Kategoriya { Nomi = "Iqtisodiyot va biznes", Tavsif = "Biznes va moliya" },
                new Kategoriya { Nomi = "Matematika", Tavsif = "Matematika va statistika" },
                new Kategoriya { Nomi = "Til va lingvistika", Tavsif = "Tilshunoslik va grammatika" },
                new Kategoriya { Nomi = "Bolalar adabiyoti", Tavsif = "Bolalar uchun kitoblar" },
                new Kategoriya { Nomi = "Dasturlash", Tavsif = "Kompyuter fanlari va dasturlash" },
                new Kategoriya { Nomi = "Tibbiyot", Tavsif = "Tibbiy adabiyot" },
                new Kategoriya { Nomi = "San'at va musiqa", Tavsif = "San'at, musiqa, kino" },
                new Kategoriya { Nomi = "Ilmiy jurnal", Tavsif = "Davriy ilmiy nashrlar" }
            );
            await ctx.SaveChangesAsync();
        }

        // ==================== MUALLIFLAR ====================
        private static async Task SeedMualliflarAsync(ApplicationDbContext ctx)
        {
            if (await ctx.Mualliflar.AnyAsync()) return;
            ctx.Mualliflar.AddRange(
                new Muallif { Ism = "Abdulla", Familiya = "Qodiriy", TugilganSana = new(1894,4,10), VafotSana = new(1938,10,4), Davlat = "O'zbekiston", Biografiya = "O'zbek romanchiligining asoschisi" },
                new Muallif { Ism = "Abdulhamid", Familiya = "Cho'lpon", TugilganSana = new(1897,1,1), VafotSana = new(1938,10,4), Davlat = "O'zbekiston", Biografiya = "O'zbek shoiri va dramaturgi" },
                new Muallif { Ism = "Hamza", Familiya = "Hakimzoda", TugilganSana = new(1889,3,6), VafotSana = new(1929,3,26), Davlat = "O'zbekiston", Biografiya = "O'zbek adabiyotining asoschisi" },
                new Muallif { Ism = "Said Ahmad", Familiya = "Husanov", TugilganSana = new(1920,1,1), VafotSana = new(2007,1,1), Davlat = "O'zbekiston", Biografiya = "O'zbek yozuvchisi" },
                new Muallif { Ism = "Erkin", Familiya = "Vohidov", TugilganSana = new(1936,12,28), VafotSana = new(2016,5,3), Davlat = "O'zbekiston", Biografiya = "O'zbek shoiri" },
                new Muallif { Ism = "Robert", Familiya = "Kiyosaki", TugilganSana = new(1947,4,8), Davlat = "AQSH", Biografiya = "Amerika tadbirkor va muallif" },
                new Muallif { Ism = "Napoleon", Familiya = "Hill", TugilganSana = new(1883,10,26), VafotSana = new(1970,11,8), Davlat = "AQSH", Biografiya = "Motivatsion muallif" },
                new Muallif { Ism = "Stiven", Familiya = "Kovi", TugilganSana = new(1932,10,24), VafotSana = new(2012,7,16), Davlat = "AQSH", Biografiya = "Amerika biznesmeni" },
                new Muallif { Ism = "Lev", Familiya = "Tolstoy", TugilganSana = new(1828,9,9), VafotSana = new(1910,11,20), Davlat = "Rossiya", Biografiya = "Rus yozuvchisi va faylasufi" },
                new Muallif { Ism = "Fyodor", Familiya = "Dostoyevskiy", TugilganSana = new(1821,11,11), VafotSana = new(1881,2,9), Davlat = "Rossiya", Biografiya = "Rus yozuvchisi" },
                new Muallif { Ism = "Paulo", Familiya = "Koelyo", TugilganSana = new(1947,8,24), Davlat = "Braziliya", Biografiya = "Braziliyalik yozuvchi" },
                new Muallif { Ism = "Jorj", Familiya = "Oruell", TugilganSana = new(1903,6,25), VafotSana = new(1950,1,21), Davlat = "Buyuk Britaniya", Biografiya = "Ingliz yozuvchisi va jurnalist" },
                new Muallif { Ism = "Arthur Konan", Familiya = "Doyl", TugilganSana = new(1859,5,22), VafotSana = new(1930,7,7), Davlat = "Buyuk Britaniya", Biografiya = "Sherlock Holmes yaratuvchisi" },
                new Muallif { Ism = "James", Familiya = "Klir", TugilganSana = new(1942,1,1), Davlat = "AQSH", Biografiya = "\"Atomic Habits\" muallifi" },
                new Muallif { Ism = "Yuval Noah", Familiya = "Xarari", TugilganSana = new(1976,2,24), Davlat = "Isroil", Biografiya = "Tarixchi va bestseller muallif" },
                new Muallif { Ism = "Robert C.", Familiya = "Martin", TugilganSana = new(1952,12,5), Davlat = "AQSH", Biografiya = "\"Uncle Bob\" — Clean Code muallifi" },
                new Muallif { Ism = "Antoine de", Familiya = "Sent-Ekzyuperi", TugilganSana = new(1900,6,29), VafotSana = new(1944,7,31), Davlat = "Fransiya", Biografiya = "\"Kichkina shahzoda\" muallifi" }
            );
            await ctx.SaveChangesAsync();
        }

        // ==================== KITOBLAR (BARCHA TURLAR) ====================
        private static async Task SeedKitoblarAsync(ApplicationDbContext ctx)
        {
            if (await ctx.Kitoblar.AnyAsync())
            {
                // Mavjud kitoblarga rasmlar qo'shish (rasm yo'q bo'lsa)
                await RasmlarniYangilashAsync(ctx);
                return;
            }

            var mualliflar = await ctx.Mualliflar.ToListAsync();
            var kategoriyalar = await ctx.Kategoriyalar.ToListAsync();

            var kitoblar = new List<(Kitob k, string[] mFam, string[] katNom)>
            {
                // ============================================================
                // KITOBLAR — O'ZBEK KLASSIKASI
                // ============================================================
                (new Kitob {
                    Nomi = "O'tkan kunlar", NashrYili = 1925, SahifalarSoni = 432, Nashriyot = "O'qituvchi",
                    ISBN = "978-9943-02-234-1", Til = "uz", Narxi = 45000, NusxalarSoni = 5, MavjudNusxalar = 3,
                    Tavsif = "O'zbek adabiyotining birinchi romani. Otabek va Kumush muhabbat qissasi.",
                    Rasm = "https://covers.openlibrary.org/b/id/10918-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.8, BaholarSoni = 124
                }, new[]{"Qodiriy"}, new[]{"Adabiyot"}),

                (new Kitob {
                    Nomi = "Mehrobdan chayon", NashrYili = 1929, SahifalarSoni = 280, Nashriyot = "O'qituvchi",
                    ISBN = "978-9943-02-235-8", Til = "uz", Narxi = 38000, NusxalarSoni = 4, MavjudNusxalar = 2,
                    Tavsif = "Abdulla Qodiriyning ikkinchi tarixi romani — Anvar va Ra'no qissasi.",
                    Rasm = "https://covers.openlibrary.org/b/id/10919-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.6, BaholarSoni = 87
                }, new[]{"Qodiriy"}, new[]{"Adabiyot"}),

                (new Kitob {
                    Nomi = "Kecha va kunduz", NashrYili = 1936, SahifalarSoni = 320, Nashriyot = "O'qituvchi",
                    ISBN = "978-9943-02-236-5", Til = "uz", Narxi = 42000, NusxalarSoni = 3, MavjudNusxalar = 1,
                    Tavsif = "Cho'lponning mashhur romani — Miryoqub va Zebi qissasi.",
                    Rasm = "https://covers.openlibrary.org/b/id/10920-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.7, BaholarSoni = 65
                }, new[]{"Cho'lpon"}, new[]{"Adabiyot"}),

                (new Kitob {
                    Nomi = "Temur tuzuklari", NashrYili = 2016, SahifalarSoni = 196, Nashriyot = "Sharq",
                    ISBN = "978-9943-02-244-0", Til = "uz", Narxi = 35000, NusxalarSoni = 3, MavjudNusxalar = 2,
                    Tavsif = "Amir Temurning davlat boshqaruvi haqidagi buyuk asari.",
                    Rasm = "https://covers.openlibrary.org/b/id/8227-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.7, BaholarSoni = 78
                }, new string[]{}, new[]{"Tarix"}),

                // ============================================================
                // KITOBLAR — XORIJ KLASSIKASI
                // ============================================================
                (new Kitob {
                    Nomi = "Urush va tinchlik", AslNomi = "Война и мир", NashrYili = 2018, SahifalarSoni = 1296,
                    Nashriyot = "O'qituvchi", ISBN = "978-9943-02-239-6", Til = "uz", Narxi = 120000,
                    NusxalarSoni = 3, MavjudNusxalar = 1,
                    Tavsif = "Lev Tolstoyning jahon adabiyotining durdonasi — Napoleon urushi davri.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0140441867-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.9, BaholarSoni = 312
                }, new[]{"Tolstoy"}, new[]{"Adabiyot", "Tarix"}),

                (new Kitob {
                    Nomi = "Jinoyat va jazo", AslNomi = "Преступление и наказание", NashrYili = 2019,
                    SahifalarSoni = 512, Nashriyot = "O'qituvchi", ISBN = "978-9943-02-240-2", Til = "uz",
                    Narxi = 85000, NusxalarSoni = 4, MavjudNusxalar = 3,
                    Tavsif = "Dostoyevskiyning psixologik romani — Raskolnikov va vijdon.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0140449136-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.8, BaholarSoni = 198
                }, new[]{"Dostoyevskiy"}, new[]{"Adabiyot", "Falsafa va psixologiya"}),

                (new Kitob {
                    Nomi = "Alkimyogar", AslNomi = "The Alchemist", NashrYili = 2020, SahifalarSoni = 208,
                    Nashriyot = "Yangi nashr", ISBN = "978-0-06-112241-5", Til = "uz", Narxi = 55000,
                    NusxalarSoni = 6, MavjudNusxalar = 4,
                    Tavsif = "Paulo Koelyo — o'z orzuingni quvish haqida dunyoga mashhur roman.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0061122416-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.7, BaholarSoni = 245
                }, new[]{"Koelyo"}, new[]{"Adabiyot", "Falsafa va psixologiya"}),

                (new Kitob {
                    Nomi = "1984", AslNomi = "Nineteen Eighty-Four", NashrYili = 2021, SahifalarSoni = 368,
                    Nashriyot = "Yangi avlod", ISBN = "978-0-451-52494-7", Til = "uz", Narxi = 72000,
                    NusxalarSoni = 4, MavjudNusxalar = 2,
                    Tavsif = "Jorj Oruell — totalitarizm haqidagi dystopiya.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0451524934-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.8, BaholarSoni = 189
                }, new[]{"Oruell"}, new[]{"Adabiyot", "Falsafa va psixologiya"}),

                (new Kitob {
                    Nomi = "Kichkina shahzoda", AslNomi = "Le Petit Prince", NashrYili = 2019, SahifalarSoni = 96,
                    Nashriyot = "O'qituvchi", ISBN = "978-0-15-601219-5", Til = "uz", Narxi = 28000,
                    NusxalarSoni = 8, MavjudNusxalar = 6,
                    Tavsif = "Sent-Ekzyuperining bolalik va sevgi haqidagi bedod asari.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0156012197-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.9, BaholarSoni = 356
                }, new[]{"Sent-Ekzyuperi"}, new[]{"Adabiyot", "Bolalar adabiyoti"}),

                (new Kitob {
                    Nomi = "Sherlock Holmes sarguzashtlari", AslNomi = "Adventures of Sherlock Holmes",
                    NashrYili = 2022, SahifalarSoni = 480, Nashriyot = "Sano-standart",
                    ISBN = "978-0-14-104043-2", Til = "uz", Narxi = 68000, NusxalarSoni = 5, MavjudNusxalar = 3,
                    Tavsif = "Konan Doylning detektiv klassikasi.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0141040432-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.6, BaholarSoni = 134
                }, new[]{"Doyl"}, new[]{"Adabiyot"}),

                // ============================================================
                // KITOBLAR — BIZNES VA SHAXSIY RIVOJLANISH
                // ============================================================
                (new Kitob {
                    Nomi = "Boy ota, kambag'al ota", AslNomi = "Rich Dad Poor Dad", NashrYili = 2020,
                    SahifalarSoni = 336, Nashriyot = "Yangi nashr", ISBN = "978-0-7432-7565-1",
                    Til = "uz", Narxi = 89000, NusxalarSoni = 6, MavjudNusxalar = 4,
                    Tavsif = "Moliyaviy mustaqillik haqidagi bestseller — Robert Kiyosaki.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0743487451-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.9, BaholarSoni = 218
                }, new[]{"Kiyosaki"}, new[]{"Iqtisodiyot va biznes"}),

                (new Kitob {
                    Nomi = "O'ylang va boyib bo'ling", AslNomi = "Think and Grow Rich",
                    NashrYili = 2019, SahifalarSoni = 285, Nashriyot = "Sano-standart",
                    ISBN = "978-1-58542-433-7", Til = "uz", Narxi = 65000, NusxalarSoni = 5, MavjudNusxalar = 3,
                    Tavsif = "Napoleon Hill — muvaffaqiyatga erishish formulasi.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/1585424331-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.7, BaholarSoni = 156
                }, new[]{"Hill"}, new[]{"Iqtisodiyot va biznes", "Falsafa va psixologiya"}),

                (new Kitob {
                    Nomi = "7 ta odatlar", AslNomi = "The 7 Habits of Highly Effective People",
                    NashrYili = 2021, SahifalarSoni = 396, Nashriyot = "Yangi avlod",
                    ISBN = "978-0-7432-9307-5", Til = "uz", Narxi = 78000, NusxalarSoni = 4, MavjudNusxalar = 2,
                    Tavsif = "Stiven Kovi — juda samarali odamlarning 7 ta odati.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0743269519-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.8, BaholarSoni = 143
                }, new[]{"Kovi"}, new[]{"Falsafa va psixologiya", "Iqtisodiyot va biznes"}),

                (new Kitob {
                    Nomi = "Sapiens: Insoniyat tarixi", AslNomi = "Sapiens: A Brief History of Humankind",
                    NashrYili = 2022, SahifalarSoni = 512, Nashriyot = "Yangi nashr",
                    ISBN = "978-0-06-231609-7", Til = "uz", Narxi = 95000, NusxalarSoni = 4, MavjudNusxalar = 3,
                    Tavsif = "Yuval Noah Xarari — insoniyatning qisqacha tarixi.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0062316095-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.8, BaholarSoni = 267
                }, new[]{"Xarari"}, new[]{"Tarix", "Fan va texnologiya"}),

                // ============================================================
                // KITOBLAR — DASTURLASH VA TEXNOLOGIYA
                // ============================================================
                (new Kitob {
                    Nomi = "Python dasturlash tili", AslNomi = "Python Crash Course",
                    NashrYili = 2022, SahifalarSoni = 480, Nashriyot = "IT-press",
                    ISBN = "978-9943-02-241-9", Til = "uz", Narxi = 95000, NusxalarSoni = 8, MavjudNusxalar = 6,
                    Tavsif = "Python dasturlash tilini bosqichma-bosqich o'rganish.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/1593279280-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.6, BaholarSoni = 89
                }, new[]{"Martin"}, new[]{"Dasturlash", "Fan va texnologiya"}),

                (new Kitob {
                    Nomi = "Toza kod", AslNomi = "Clean Code",
                    NashrYili = 2020, SahifalarSoni = 431, Nashriyot = "IT-press",
                    ISBN = "978-0-13-235088-4", Til = "uz", Narxi = 110000, NusxalarSoni = 5, MavjudNusxalar = 3,
                    Tavsif = "Robert C. Martin — professional dasturchi uchun kod yozish qo'llanmasi.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0132350882-M.jpg",
                    MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.7, BaholarSoni = 112
                }, new[]{"Martin"}, new[]{"Dasturlash"}),

                // ============================================================
                // ELEKTRON KITOBLAR (ElektronKitob)
                // ============================================================
                (new Kitob {
                    Nomi = "JavaScript: The Good Parts (Elektron)", AslNomi = "JavaScript: The Good Parts",
                    NashrYili = 2023, SahifalarSoni = 176, Nashriyot = "O'Reilly",
                    ISBN = "978-0-596-51774-8", Til = "en", Narxi = 45000, NusxalarSoni = 100, MavjudNusxalar = 100,
                    Tavsif = "JavaScript tilining eng yaxshi tomonlari. To'liq elektron format.",
                    Annotatsiya = "Bu kitob elektron formatda mavjud. Yuklab olish va onlayn o'qish mumkin.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0596517742-M.jpg",
                    MediaTuri = Kitob.MediaType.ElektronKitob, Elektron = true,
                    ElektronManzil = "https://example.com/books/js-good-parts.pdf",
                    FaylFormati = "PDF", OrtachaBaho = 4.5, BaholarSoni = 78
                }, new string[]{}, new[]{"Dasturlash"}),

                (new Kitob {
                    Nomi = "Atomic Habits (Elektron)", AslNomi = "Atomic Habits",
                    NashrYili = 2023, SahifalarSoni = 320, Nashriyot = "Avery",
                    ISBN = "978-0-73521-129-2", Til = "uz", Narxi = 49000, NusxalarSoni = 100, MavjudNusxalar = 100,
                    Tavsif = "James Klir — kichik odatlar orqali katta natijalarga erishish.",
                    Annotatsiya = "Elektron kitob. PDF va EPUB formatlarida mavjud.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0735211299-M.jpg",
                    MediaTuri = Kitob.MediaType.ElektronKitob, Elektron = true,
                    ElektronManzil = "https://example.com/books/atomic-habits.pdf",
                    FaylFormati = "PDF,EPUB", OrtachaBaho = 4.9, BaholarSoni = 203
                }, new[]{"Klir"}, new[]{"Falsafa va psixologiya", "Iqtisodiyot va biznes"}),

                (new Kitob {
                    Nomi = "O'zbek tili grammatikasi (Elektron)", NashrYili = 2022,
                    SahifalarSoni = 520, Nashriyot = "Fan", ISBN = "978-9943-02-250-1",
                    Til = "uz", Narxi = 0, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "O'zbek tilining to'liq grammatikasi. Bepul elektron nashr.",
                    Annotatsiya = "O'zbekiston Fanlar Akademiyasi nashriyotining elektron versiyasi.",
                    Rasm = "https://covers.openlibrary.org/b/id/8228-M.jpg",
                    MediaTuri = Kitob.MediaType.ElektronKitob, Elektron = true,
                    ElektronManzil = "https://example.com/books/uzbek-grammar.pdf",
                    FaylFormati = "PDF", OrtachaBaho = 4.4, BaholarSoni = 34
                }, new string[]{}, new[]{"Til va lingvistika"}),

                (new Kitob {
                    Nomi = "Yuqori matematik analiz (Elektron)", NashrYili = 2021,
                    SahifalarSoni = 680, Nashriyot = "Fan", ISBN = "978-9943-02-251-8",
                    Til = "uz", Narxi = 25000, NusxalarSoni = 50, MavjudNusxalar = 50,
                    Tavsif = "Oliy matematika kursi — integrallar, differentsial tenglamalar.",
                    Rasm = "https://covers.openlibrary.org/b/id/8229-M.jpg",
                    MediaTuri = Kitob.MediaType.ElektronKitob, Elektron = true,
                    ElektronManzil = "https://example.com/books/higher-math.pdf",
                    FaylFormati = "PDF", OrtachaBaho = 4.1, BaholarSoni = 22
                }, new string[]{}, new[]{"Matematika"}),

                (new Kitob {
                    Nomi = "Data Science bilan tanishuv (Elektron)", AslNomi = "Intro to Data Science",
                    NashrYili = 2023, SahifalarSoni = 392, Nashriyot = "O'Reilly",
                    ISBN = "978-1-491-90205-5", Til = "en", Narxi = 69000, NusxalarSoni = 50, MavjudNusxalar = 50,
                    Tavsif = "Ma'lumotlar tahlili va machine learning asoslari.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/1491902056-M.jpg",
                    MediaTuri = Kitob.MediaType.ElektronKitob, Elektron = true,
                    ElektronManzil = "https://example.com/books/data-science.pdf",
                    FaylFormati = "PDF,EPUB", OrtachaBaho = 4.6, BaholarSoni = 67
                }, new string[]{}, new[]{"Dasturlash", "Fan va texnologiya"}),

                // ============================================================
                // AUDIO KITOBLAR (AudioKitob)
                // ============================================================
                (new Kitob {
                    Nomi = "Boy ota kambag'al ota (Audio)", AslNomi = "Rich Dad Poor Dad Audio",
                    NashrYili = 2022, Nashriyot = "AudioBooks UZ",
                    ISBN = "978-0-743-48745-0", Til = "uz", Narxi = 35000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Robert Kiyosaki kitobining o'zbek tilidagi audio versiyasi. Muallif o'qi ovozi bilan.",
                    Annotatsiya = "Umumiy davomiyligi: 6 soat 20 daqiqa. MP3 formatida.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0743487451-M.jpg",
                    MediaTuri = Kitob.MediaType.AudioKitob,
                    ElektronManzil = "https://example.com/audio/rich-dad-poor-dad.mp3",
                    FaylFormati = "MP3", OrtachaBaho = 4.7, BaholarSoni = 89
                }, new[]{"Kiyosaki"}, new[]{"Iqtisodiyot va biznes"}),

                (new Kitob {
                    Nomi = "Alkimyogar (Audio)", AslNomi = "The Alchemist Audio",
                    NashrYili = 2022, Nashriyot = "AudioBooks UZ",
                    ISBN = "978-0-06-112241-6", Til = "uz", Narxi = 25000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Paulo Koelyo kitobining audio versiyasi. Davomiyligi: 4 soat 15 daqiqa.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0061122416-M.jpg",
                    MediaTuri = Kitob.MediaType.AudioKitob,
                    ElektronManzil = "https://example.com/audio/alchemist.mp3",
                    FaylFormati = "MP3", OrtachaBaho = 4.8, BaholarSoni = 112
                }, new[]{"Koelyo"}, new[]{"Adabiyot"}),

                (new Kitob {
                    Nomi = "Atomic Habits (Audio)", AslNomi = "Atomic Habits Audio",
                    NashrYili = 2023, Nashriyot = "AudioBooks UZ",
                    ISBN = "978-0-73521-129-3", Til = "uz", Narxi = 30000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "James Klir kitobining audio versiyasi. Davomiyligi: 5 soat 35 daqiqa.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0735211299-M.jpg",
                    MediaTuri = Kitob.MediaType.AudioKitob,
                    ElektronManzil = "https://example.com/audio/atomic-habits.mp3",
                    FaylFormati = "MP3,AAC", OrtachaBaho = 4.9, BaholarSoni = 156
                }, new[]{"Klir"}, new[]{"Falsafa va psixologiya"}),

                (new Kitob {
                    Nomi = "O'tkan kunlar (Audio)", NashrYili = 2021, Nashriyot = "O'zovoz studio",
                    ISBN = "978-9943-02-260-0", Til = "uz", Narxi = 20000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Abdulla Qodiriyning mashhur romanining audio versiyasi. O'zbek aktyor ovozi.",
                    Annotatsiya = "Davomiyligi: 12 soat. Professional studiyada yozilgan.",
                    Rasm = "https://covers.openlibrary.org/b/id/10918-M.jpg",
                    MediaTuri = Kitob.MediaType.AudioKitob,
                    ElektronManzil = "https://example.com/audio/otkan-kunlar.mp3",
                    FaylFormati = "MP3", OrtachaBaho = 4.8, BaholarSoni = 76
                }, new[]{"Qodiriy"}, new[]{"Adabiyot"}),

                (new Kitob {
                    Nomi = "Sapiens: Audio", AslNomi = "Sapiens Audio",
                    NashrYili = 2023, Nashriyot = "AudioBooks UZ",
                    ISBN = "978-0-06-231609-8", Til = "uz", Narxi = 39000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Yuval Noah Xarari kitobining audio versiyasi. Davomiyligi: 15 soat.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/0062316095-M.jpg",
                    MediaTuri = Kitob.MediaType.AudioKitob,
                    ElektronManzil = "https://example.com/audio/sapiens.mp3",
                    FaylFormati = "MP3,AAC", OrtachaBaho = 4.7, BaholarSoni = 98
                }, new[]{"Xarari"}, new[]{"Tarix"}),

                // ============================================================
                // JURNALLAR (Jurnal)
                // ============================================================
                (new Kitob {
                    Nomi = "O'zbek tili va adabiyoti — 2024/1", NashrYili = 2024, SahifalarSoni = 128,
                    Nashriyot = "Fanlar akademiyasi", ISBN = "ISSN 2181-7952", Til = "uz",
                    Narxi = 12000, NusxalarSoni = 10, MavjudNusxalar = 8,
                    Tavsif = "O'zbek tili va adabiyoti bo'yicha ilmiy jurnal. 2024-yil 1-son.",
                    Annotatsiya = "Maqolalar: lingvistika, adabiyotshunoslik, metodika.",
                    Rasm = "https://covers.openlibrary.org/b/id/9222-M.jpg",
                    MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.2, BaholarSoni = 12
                }, new string[]{}, new[]{"Ilmiy jurnal", "Til va lingvistika"}),

                (new Kitob {
                    Nomi = "Matematika va informatika — 2024/2", NashrYili = 2024, SahifalarSoni = 112,
                    Nashriyot = "Toshkent DPI", ISBN = "ISSN 2181-8843", Til = "uz",
                    Narxi = 10000, NusxalarSoni = 8, MavjudNusxalar = 6,
                    Tavsif = "Matematika va informatika bo'yicha ilmiy-uslubiy jurnal.",
                    Rasm = "https://covers.openlibrary.org/b/id/9223-M.jpg",
                    MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.0, BaholarSoni = 8
                }, new string[]{}, new[]{"Ilmiy jurnal", "Matematika"}),

                (new Kitob {
                    Nomi = "Iqtisodiyot va moliya — 2024/1", NashrYili = 2024, SahifalarSoni = 96,
                    Nashriyot = "TDIU", ISBN = "ISSN 2181-9297", Til = "uz",
                    Narxi = 11000, NusxalarSoni = 6, MavjudNusxalar = 5,
                    Tavsif = "Iqtisodiyot va moliya tizimi bo'yicha ilmiy jurnal.",
                    Rasm = "https://covers.openlibrary.org/b/id/9224-M.jpg",
                    MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.1, BaholarSoni = 9
                }, new string[]{}, new[]{"Ilmiy jurnal", "Iqtisodiyot va biznes"}),

                (new Kitob {
                    Nomi = "Tibbiyot xabarnomasi — 2024/3", NashrYili = 2024, SahifalarSoni = 140,
                    Nashriyot = "ToshDTU", ISBN = "ISSN 2181-7928", Til = "uz",
                    Narxi = 14000, NusxalarSoni = 5, MavjudNusxalar = 4,
                    Tavsif = "Tibbiyot fanlari bo'yicha ilmiy-amaliy jurnal.",
                    Rasm = "https://covers.openlibrary.org/b/id/9225-M.jpg",
                    MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.3, BaholarSoni = 7
                }, new string[]{}, new[]{"Ilmiy jurnal", "Tibbiyot"}),

                (new Kitob {
                    Nomi = "Yoshlar dunyosi — 2024 aprel", NashrYili = 2024, SahifalarSoni = 80,
                    Nashriyot = "Yoshlar nashriyoti", ISBN = "ISSN 0236-2082", Til = "uz",
                    Narxi = 8000, NusxalarSoni = 15, MavjudNusxalar = 12,
                    Tavsif = "Yoshlarga mo'ljallangan ommabop ilmiy-ommaviy jurnal.",
                    Annotatsiya = "Ilm-fan, texnologiya, madaniyat, sport yangiliklari.",
                    Rasm = "https://covers.openlibrary.org/b/id/9226-M.jpg",
                    MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.4, BaholarSoni = 28
                }, new string[]{}, new[]{"Ilmiy jurnal"}),

                // ============================================================
                // VIDEO MATERIALLAR (Video)
                // ============================================================
                (new Kitob {
                    Nomi = "Python dasturlash — to'liq video kurs", NashrYili = 2023,
                    Nashriyot = "IT Academy UZ", ISBN = "978-9943-02-270-9", Til = "uz",
                    Narxi = 150000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Python dasturlash tilini noldan professional darajagacha o'rgatuvchi video kurs.",
                    Annotatsiya = "105 ta dars, 40+ soat video. Mavzular: Python asoslari, OOP, Django, Data Science.",
                    Rasm = "https://covers.openlibrary.org/b/isbn/1593279280-M.jpg",
                    MediaTuri = Kitob.MediaType.Video,
                    ElektronManzil = "https://example.com/video/python-full-course",
                    FaylFormati = "MP4", OrtachaBaho = 4.8, BaholarSoni = 145
                }, new string[]{}, new[]{"Dasturlash", "Fan va texnologiya"}),

                (new Kitob {
                    Nomi = "O'zbek tarixi — video ma'ruzalar", NashrYili = 2022,
                    Nashriyot = "O'zDU video studio", ISBN = "978-9943-02-271-6", Til = "uz",
                    Narxi = 0, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "O'zbekiston tarixi bo'yicha to'liq video ma'ruzalar kursi. Bepul.",
                    Annotatsiya = "50 ta ma'ruza, 35 soat. Qadimgi davr, O'rta asrlar, Zamonaviy tarix.",
                    Rasm = "https://covers.openlibrary.org/b/id/8227-M.jpg",
                    MediaTuri = Kitob.MediaType.Video,
                    ElektronManzil = "https://example.com/video/uzbek-history",
                    FaylFormati = "MP4", OrtachaBaho = 4.6, BaholarSoni = 89
                }, new string[]{}, new[]{"Tarix"}),

                (new Kitob {
                    Nomi = "Ingliz tili — A1 dan C1 gacha", NashrYili = 2023,
                    Nashriyot = "Language School UZ", ISBN = "978-9943-02-272-3", Til = "uz",
                    Narxi = 199000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Ingliz tilini bosqichma-bosqich o'rgatuvchi to'liq video kurs.",
                    Annotatsiya = "200+ dars, 80 soat video. Grammar, Speaking, Listening, Writing.",
                    Rasm = "https://covers.openlibrary.org/b/id/8230-M.jpg",
                    MediaTuri = Kitob.MediaType.Video,
                    ElektronManzil = "https://example.com/video/english-course",
                    FaylFormati = "MP4", OrtachaBaho = 4.9, BaholarSoni = 234
                }, new string[]{}, new[]{"Til va lingvistika"}),

                (new Kitob {
                    Nomi = "Matematika olympiada tayyorlov kursi", NashrYili = 2023,
                    Nashriyot = "Iqtidor video", ISBN = "978-9943-02-273-0", Til = "uz",
                    Narxi = 120000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Matematika olympiadalariga tayyorlanish uchun maxsus video kurs.",
                    Annotatsiya = "80 ta mashg'ulot. Algebra, geometriya, kombinatorika, sonlar nazariyasi.",
                    Rasm = "https://covers.openlibrary.org/b/id/8231-M.jpg",
                    MediaTuri = Kitob.MediaType.Video,
                    ElektronManzil = "https://example.com/video/math-olympiad",
                    FaylFormati = "MP4", OrtachaBaho = 4.7, BaholarSoni = 67
                }, new string[]{}, new[]{"Matematika"}),

                (new Kitob {
                    Nomi = "Grafik dizayn — Photoshop & Illustrator", NashrYili = 2023,
                    Nashriyot = "Creative UZ", ISBN = "978-9943-02-274-7", Til = "uz",
                    Narxi = 180000, NusxalarSoni = 999, MavjudNusxalar = 999,
                    Tavsif = "Adobe Photoshop va Illustrator dasturlarini o'rgatuvchi video kurs.",
                    Annotatsiya = "150 ta dars, 55 soat. Boshlang'ichdan professional darajagacha.",
                    Rasm = "https://covers.openlibrary.org/b/id/8232-M.jpg",
                    MediaTuri = Kitob.MediaType.Video,
                    ElektronManzil = "https://example.com/video/graphic-design",
                    FaylFormati = "MP4", OrtachaBaho = 4.8, BaholarSoni = 123
                }, new string[]{}, new[]{"San'at va musiqa", "Fan va texnologiya"}),
            };

            foreach (var (kitob, mFamiliyalar, katNomilar) in kitoblar)
            {
                ctx.Kitoblar.Add(kitob);
                await ctx.SaveChangesAsync();

                foreach (var fam in mFamiliyalar)
                {
                    var m = mualliflar.FirstOrDefault(mu => mu.Familiya.Contains(fam) || mu.Ism.Contains(fam));
                    if (m != null && !await ctx.KitobMualliflari.AnyAsync(km => km.KitobId == kitob.Id && km.MuallifId == m.Id))
                        ctx.KitobMualliflari.Add(new KitobMuallif { KitobId = kitob.Id, MuallifId = m.Id });
                }

                foreach (var katNomi in katNomilar)
                {
                    var k = kategoriyalar.FirstOrDefault(ka => ka.Nomi == katNomi);
                    if (k != null && !await ctx.KitobKategoriyalari.AnyAsync(kk => kk.KitobId == kitob.Id && kk.KategoriyaId == k.Id))
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = kitob.Id, KategoriyaId = k.Id });
                }

                // Faqat jismoniy kitoblarga nusxalar (elektron/audio/video ga emas)
                bool jismoniy = kitob.MediaTuri == Kitob.MediaType.Kitob || kitob.MediaTuri == Kitob.MediaType.Jurnal;
                if (jismoniy)
                {
                    for (int i = 1; i <= kitob.NusxalarSoni && i <= 10; i++)
                    {
                        // Har bir kitobning 1-nusxasi sotuvga qo'yiladi (agar narxi bo'lsa)
                        bool sotuvda = i == 1 && kitob.Narxi.HasValue && kitob.Narxi > 0;
                        ctx.KitobNusxalari.Add(new KitobNusxa
                        {
                            KitobId = kitob.Id,
                            InventarRaqami = $"INV-{kitob.Id:D4}-{i:D2}",
                            BarKod = $"BRK{kitob.Id:D6}{i:D2}",
                            Holati = i <= kitob.MavjudNusxalar ? "Mavjud" : "Berilgan",
                            JismoniyHolati = i == 1 ? "Yangi" : "Yaxshi",
                            QabulQilinganSana = DateTime.Now.AddMonths(-Random.Shared.Next(1, 24)),
                            KelibChiqishi = "Sotib olingan",
                            Joylashuv = $"A-{Random.Shared.Next(1, 10)}-{Random.Shared.Next(1, 30)}",
                            Narxi = kitob.Narxi,
                            SotuvgaQoyilgan = sotuvda
                        });
                    }
                }
                else
                {
                    // Raqamli materiallar uchun 1 ta nusxa
                    ctx.KitobNusxalari.Add(new KitobNusxa
                    {
                        KitobId = kitob.Id,
                        InventarRaqami = $"DIG-{kitob.Id:D4}-01",
                        BarKod = $"DIG{kitob.Id:D6}01",
                        Holati = "Mavjud",
                        JismoniyHolati = "Yangi",
                        QabulQilinganSana = DateTime.Now.AddMonths(-Random.Shared.Next(1, 12)),
                        KelibChiqishi = "Raqamli material",
                        FaylManzili = kitob.ElektronManzil,
                        FaylFormati = kitob.FaylFormati,
                        YuklabOlishMumkin = true,
                        Narxi = kitob.Narxi
                    });
                }

                await ctx.SaveChangesAsync();
            }
        }

        // ==================== TEST MATERIALLAR (har turdan >= 4 ta) ====================
        private static async Task SeedTestMateriallarAsync(ApplicationDbContext ctx)
        {
            const int MIN = 4; // har tur uchun minimum

            // Mualliflar va kategoriyalarni yuklaymiz (mavjud yoki yaratamiz)
            var defaultMuallif = await ctx.Mualliflar.FirstOrDefaultAsync();
            if (defaultMuallif == null) return; // mualliflar yo'q bo'lsa chiqamiz

            var defaultKat = await ctx.Kategoriyalar.FirstOrDefaultAsync();

            // ===== JISMONIY KITOBLAR =====
            int kitobCount = await ctx.Kitoblar.CountAsync(k => k.MediaTuri == Kitob.MediaType.Kitob);
            if (kitobCount < MIN)
            {
                var yangiKitoblar = new[]
                {
                    new Kitob { Nomi = "Baxtiyor va uning do'stlari", NashrYili = 2020, SahifalarSoni = 248,
                        Nashriyot = "Yangi nashr", ISBN = "978-9943-11-001-1", Til = "uz",
                        Narxi = 35000, NusxalarSoni = 5, MavjudNusxalar = 4,
                        Tavsif = "Zamonaviy o'zbek adabiyotining eng sara asarlaridan biri.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739161-M.jpg",
                        MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.3, BaholarSoni = 18 },

                    new Kitob { Nomi = "Vatan haqida qo'shiqlar", NashrYili = 2019, SahifalarSoni = 180,
                        Nashriyot = "Adabiyot nashriyoti", ISBN = "978-9943-11-002-2", Til = "uz",
                        Narxi = 28000, NusxalarSoni = 6, MavjudNusxalar = 5,
                        Tavsif = "O'zbek shoirlari she'rlari to'plami.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739162-M.jpg",
                        MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.5, BaholarSoni = 32 },

                    new Kitob { Nomi = "Iqtisodiyot asoslari", NashrYili = 2022, SahifalarSoni = 420,
                        Nashriyot = "Universitet nashriyoti", ISBN = "978-9943-11-003-3", Til = "uz",
                        Narxi = 55000, NusxalarSoni = 8, MavjudNusxalar = 6,
                        Tavsif = "Iqtisodiyot nazariyasi va amaliyoti bo'yicha darslik.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739163-M.jpg",
                        MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.1, BaholarSoni = 45 },

                    new Kitob { Nomi = "Informatika va dasturlash", NashrYili = 2023, SahifalarSoni = 350,
                        Nashriyot = "IT Press", ISBN = "978-9943-11-004-4", Til = "uz",
                        Narxi = 48000, NusxalarSoni = 7, MavjudNusxalar = 5,
                        Tavsif = "Maktab va talabalar uchun informatika bo'yicha to'liq qo'llanma.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739164-M.jpg",
                        MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.6, BaholarSoni = 57 },

                    new Kitob { Nomi = "Tarix darsligi", NashrYili = 2021, SahifalarSoni = 310,
                        Nashriyot = "Maktab nashriyoti", ISBN = "978-9943-11-005-5", Til = "uz",
                        Narxi = 32000, NusxalarSoni = 10, MavjudNusxalar = 8,
                        Tavsif = "O'rta maktablar uchun O'zbekiston tarixi darsligi.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739165-M.jpg",
                        MediaTuri = Kitob.MediaType.Kitob, OrtachaBaho = 4.4, BaholarSoni = 63 },
                };

                int kerak = MIN - kitobCount;
                foreach (var k in yangiKitoblar.Take(Math.Max(kerak, yangiKitoblar.Length)))
                {
                    if (await ctx.Kitoblar.AnyAsync(x => x.ISBN == k.ISBN)) continue;
                    ctx.Kitoblar.Add(k);
                    await ctx.SaveChangesAsync();
                    if (defaultMuallif != null)
                        ctx.KitobMualliflari.Add(new KitobMuallif { KitobId = k.Id, MuallifId = defaultMuallif.Id });
                    if (defaultKat != null)
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = k.Id, KategoriyaId = defaultKat.Id });
                    // Jismoniy nusxalar (1-nusxa sotuvga qo'yiladi)
                    for (int i = 1; i <= k.NusxalarSoni && i <= 8; i++)
                    {
                        ctx.KitobNusxalari.Add(new KitobNusxa
                        {
                            KitobId = k.Id,
                            InventarRaqami = $"INV-T{k.Id:D4}-{i:D2}",
                            BarKod = $"TST{k.Id:D6}{i:D2}",
                            Holati = i <= k.MavjudNusxalar ? "Mavjud" : "Berilgan",
                            JismoniyHolati = i == 1 ? "Yangi" : "Yaxshi",
                            QabulQilinganSana = DateTime.Now.AddMonths(-i),
                            KelibChiqishi = "Sotib olingan",
                            Narxi = k.Narxi,
                            SotuvgaQoyilgan = i == 1
                        });
                    }
                    await ctx.SaveChangesAsync();
                }
            }

            // ===== JURNALLAR =====
            int jurnalCount = await ctx.Kitoblar.CountAsync(k => k.MediaTuri == Kitob.MediaType.Jurnal);
            if (jurnalCount < MIN)
            {
                var yangiJurnallar = new[]
                {
                    new Kitob { Nomi = "Fan va hayot — 2024/1", NashrYili = 2024, SahifalarSoni = 96,
                        Nashriyot = "Fan nashriyoti", ISBN = "ISSN-TEST-J001", Til = "uz",
                        Narxi = 12000, NusxalarSoni = 8, MavjudNusxalar = 7,
                        Tavsif = "Ommabop fan va texnologiya jurnali. Har oyda chiqadi.",
                        Rasm = "https://covers.openlibrary.org/b/id/9265490-M.jpg",
                        MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.2, BaholarSoni = 11 },

                    new Kitob { Nomi = "Iqtisod xabarlari — 2024/2", NashrYili = 2024, SahifalarSoni = 80,
                        Nashriyot = "Biznes nashriyoti", ISBN = "ISSN-TEST-J002", Til = "uz",
                        Narxi = 15000, NusxalarSoni = 6, MavjudNusxalar = 5,
                        Tavsif = "Iqtisodiyot, biznes va moliya yangiliklari jurnali.",
                        Rasm = "https://covers.openlibrary.org/b/id/9265491-M.jpg",
                        MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.0, BaholarSoni = 8 },

                    new Kitob { Nomi = "Sog'liq va tibbiyot — 2024/3", NashrYili = 2024, SahifalarSoni = 72,
                        Nashriyot = "Salomatliq nashriyoti", ISBN = "ISSN-TEST-J003", Til = "uz",
                        Narxi = 10000, NusxalarSoni = 5, MavjudNusxalar = 4,
                        Tavsif = "Sog'lom turmush tarzi va tibbiy maslahatlar jurnali.",
                        Rasm = "https://covers.openlibrary.org/b/id/9265492-M.jpg",
                        MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.3, BaholarSoni = 14 },

                    new Kitob { Nomi = "Adabiyot va san'at — 2024/4", NashrYili = 2024, SahifalarSoni = 88,
                        Nashriyot = "Madaniyat nashriyoti", ISBN = "ISSN-TEST-J004", Til = "uz",
                        Narxi = 11000, NusxalarSoni = 7, MavjudNusxalar = 6,
                        Tavsif = "O'zbek adabiyoti, musiqa va san'at jurnali.",
                        Rasm = "https://covers.openlibrary.org/b/id/9265493-M.jpg",
                        MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.5, BaholarSoni = 19 },

                    new Kitob { Nomi = "Yoshlar dunyosi — 2024/5", NashrYili = 2024, SahifalarSoni = 64,
                        Nashriyot = "Yoshlar nashriyoti", ISBN = "ISSN-TEST-J005", Til = "uz",
                        Narxi = 8000, NusxalarSoni = 10, MavjudNusxalar = 9,
                        Tavsif = "Yoshlarga mo'ljallangan ommabop jurnal.",
                        Rasm = "https://covers.openlibrary.org/b/id/9265494-M.jpg",
                        MediaTuri = Kitob.MediaType.Jurnal, OrtachaBaho = 4.4, BaholarSoni = 22 },
                };

                int kerak = MIN - jurnalCount;
                foreach (var k in yangiJurnallar.Take(Math.Max(kerak, yangiJurnallar.Length)))
                {
                    if (await ctx.Kitoblar.AnyAsync(x => x.ISBN == k.ISBN)) continue;
                    ctx.Kitoblar.Add(k);
                    await ctx.SaveChangesAsync();
                    if (defaultKat != null)
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = k.Id, KategoriyaId = defaultKat.Id });
                    for (int i = 1; i <= k.NusxalarSoni && i <= 8; i++)
                    {
                        ctx.KitobNusxalari.Add(new KitobNusxa
                        {
                            KitobId = k.Id,
                            InventarRaqami = $"JRN-T{k.Id:D4}-{i:D2}",
                            BarKod = $"JRN{k.Id:D6}{i:D2}",
                            Holati = i <= k.MavjudNusxalar ? "Mavjud" : "Berilgan",
                            JismoniyHolati = "Yaxshi",
                            QabulQilinganSana = DateTime.Now.AddMonths(-i),
                            KelibChiqishi = "Yuborilgan",
                            Narxi = k.Narxi,
                            SotuvgaQoyilgan = i == 1
                        });
                    }
                    await ctx.SaveChangesAsync();
                }
            }

            // ===== ELEKTRON KITOBLAR =====
            int eKitobCount = await ctx.Kitoblar.CountAsync(k => k.MediaTuri == Kitob.MediaType.ElektronKitob);
            if (eKitobCount < MIN)
            {
                var yangiEKitoblar = new[]
                {
                    new Kitob { Nomi = "C# dasturlash qo'llanmasi (PDF)", NashrYili = 2023, SahifalarSoni = 520,
                        Nashriyot = "Digital Books UZ", ISBN = "EBOOK-TEST-001", Til = "uz",
                        Narxi = 75000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "C# dasturlash tilini noldan o'rgatuvchi to'liq elektron qo'llanma.",
                        Annotatsiya = "OOP, LINQ, async/await, Entity Framework va ASP.NET Core mavzularini qamrab oladi.",
                        Rasm = "https://covers.openlibrary.org/b/isbn/0596517742-M.jpg",
                        MediaTuri = Kitob.MediaType.ElektronKitob,
                        Elektron = true, ElektronManzil = "https://example.com/ebook/csharp-guide.pdf",
                        FaylFormati = "PDF", OrtachaBaho = 4.8, BaholarSoni = 93 },

                    new Kitob { Nomi = "HTML va CSS — zamonaviy web (PDF)", NashrYili = 2023, SahifalarSoni = 380,
                        Nashriyot = "WebDev UZ", ISBN = "EBOOK-TEST-002", Til = "uz",
                        Narxi = 55000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Web dizayn va frontend dasturlash bo'yicha elektron darslik.",
                        Annotatsiya = "HTML5, CSS3, Flexbox, Grid, Bootstrap, responsive dizayn.",
                        Rasm = "https://covers.openlibrary.org/b/isbn/0735619670-M.jpg",
                        MediaTuri = Kitob.MediaType.ElektronKitob,
                        Elektron = true, ElektronManzil = "https://example.com/ebook/html-css.pdf",
                        FaylFormati = "PDF", OrtachaBaho = 4.6, BaholarSoni = 72 },

                    new Kitob { Nomi = "Sun'iy intellekt: asoslar (EPUB)", NashrYili = 2024, SahifalarSoni = 460,
                        Nashriyot = "AI Books", ISBN = "EBOOK-TEST-003", Til = "uz",
                        Narxi = 90000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "AI va machine learning bo'yicha zamonaviy elektron kitob.",
                        Annotatsiya = "ML asoslari, neural networks, Python bilan amaliyot.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739178-M.jpg",
                        MediaTuri = Kitob.MediaType.ElektronKitob,
                        Elektron = true, ElektronManzil = "https://example.com/ebook/ai-basics.epub",
                        FaylFormati = "EPUB", OrtachaBaho = 4.9, BaholarSoni = 108 },

                    new Kitob { Nomi = "O'zbek tili grammatikasi (PDF)", NashrYili = 2022, SahifalarSoni = 290,
                        Nashriyot = "Lingvistika UZ", ISBN = "EBOOK-TEST-004", Til = "uz",
                        Narxi = 40000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "O'zbek tili morfologiyasi va sintaksisi bo'yicha to'liq elektron qo'llanma.",
                        Annotatsiya = "Maktab va oliy o'quv yurti talabalari uchun.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739179-M.jpg",
                        MediaTuri = Kitob.MediaType.ElektronKitob,
                        Elektron = true, ElektronManzil = "https://example.com/ebook/uzbek-grammar.pdf",
                        FaylFormati = "PDF", OrtachaBaho = 4.5, BaholarSoni = 61 },

                    new Kitob { Nomi = "Biznes va menejment (PDF)", NashrYili = 2023, SahifalarSoni = 340,
                        Nashriyot = "Business Books UZ", ISBN = "EBOOK-TEST-005", Til = "uz",
                        Narxi = 65000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Zamonaviy biznes yuritish va boshqaruv asoslari.",
                        Annotatsiya = "Startap yaratish, marketing, moliyaviy boshqaruv.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739180-M.jpg",
                        MediaTuri = Kitob.MediaType.ElektronKitob,
                        Elektron = true, ElektronManzil = "https://example.com/ebook/business-management.pdf",
                        FaylFormati = "PDF", OrtachaBaho = 4.7, BaholarSoni = 84 },
                };

                foreach (var k in yangiEKitoblar)
                {
                    if (await ctx.Kitoblar.AnyAsync(x => x.ISBN == k.ISBN)) continue;
                    ctx.Kitoblar.Add(k);
                    await ctx.SaveChangesAsync();
                    if (defaultKat != null)
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = k.Id, KategoriyaId = defaultKat.Id });
                    // Digital nusxa — sotuvga qo'yilgan
                    ctx.KitobNusxalari.Add(new KitobNusxa
                    {
                        KitobId = k.Id,
                        InventarRaqami = $"DIG-T{k.Id:D4}-01",
                        BarKod = $"DIG{k.Id:D6}01",
                        Holati = "Mavjud", JismoniyHolati = "Yangi",
                        QabulQilinganSana = DateTime.Now.AddMonths(-1),
                        KelibChiqishi = "Raqamli material",
                        FaylManzili = k.ElektronManzil, FaylFormati = k.FaylFormati,
                        YuklabOlishMumkin = true, Narxi = k.Narxi,
                        SotuvgaQoyilgan = true
                    });
                    await ctx.SaveChangesAsync();
                }
            }

            // ===== AUDIO KITOBLAR =====
            int audioCount = await ctx.Kitoblar.CountAsync(k => k.MediaTuri == Kitob.MediaType.AudioKitob);
            if (audioCount < MIN)
            {
                var yangiAudiolar = new[]
                {
                    new Kitob { Nomi = "O'tkan kunlar — audio kitob", NashrYili = 2022, SahifalarSoni = 0,
                        Nashriyot = "Audio Books UZ", ISBN = "AUDIO-TEST-001", Til = "uz",
                        Narxi = 30000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Abdulla Qodiriy romani professional aktyor ijrosida.",
                        Annotatsiya = "12 soat 40 daqiqa. Sifatli ovoz yozuvi.",
                        Rasm = "https://covers.openlibrary.org/b/id/10918-M.jpg",
                        MediaTuri = Kitob.MediaType.AudioKitob,
                        Elektron = true, ElektronManzil = "https://example.com/audio/otkan-kunlar.mp3",
                        FaylFormati = "MP3", OrtachaBaho = 4.9, BaholarSoni = 76 },

                    new Kitob { Nomi = "Motivatsiya sirlari — audio", NashrYili = 2023, SahifalarSoni = 0,
                        Nashriyot = "MindPower UZ", ISBN = "AUDIO-TEST-002", Til = "uz",
                        Narxi = 25000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Muvaffaqiyatga erishish va o'z-o'zini rivojlantirish haqida audio kitob.",
                        Annotatsiya = "8 soat 20 daqiqa. Amaliy mashqlar bilan.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739183-M.jpg",
                        MediaTuri = Kitob.MediaType.AudioKitob,
                        Elektron = true, ElektronManzil = "https://example.com/audio/motivation.mp3",
                        FaylFormati = "MP3", OrtachaBaho = 4.7, BaholarSoni = 54 },

                    new Kitob { Nomi = "Ingliz tili — tinglash mashqlari", NashrYili = 2023, SahifalarSoni = 0,
                        Nashriyot = "LinguaUZ", ISBN = "AUDIO-TEST-003", Til = "uz",
                        Narxi = 35000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Ingliz tilini tinglash orqali o'rganish uchun audio kurs.",
                        Annotatsiya = "50 ta audio dars, 15 soat. A1 dan B2 gacha.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739184-M.jpg",
                        MediaTuri = Kitob.MediaType.AudioKitob,
                        Elektron = true, ElektronManzil = "https://example.com/audio/english-listening.mp3",
                        FaylFormati = "MP3", OrtachaBaho = 4.8, BaholarSoni = 89 },

                    new Kitob { Nomi = "Psixologiya — o'zingizni biling", NashrYili = 2022, SahifalarSoni = 0,
                        Nashriyot = "PsyBooks UZ", ISBN = "AUDIO-TEST-004", Til = "uz",
                        Narxi = 28000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Psixologiya asoslari va shaxsiyatni o'rganish haqida audio kitob.",
                        Annotatsiya = "10 soat 15 daqiqa. Ilmiy-ommabop uslubda.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739185-M.jpg",
                        MediaTuri = Kitob.MediaType.AudioKitob,
                        Elektron = true, ElektronManzil = "https://example.com/audio/psychology.mp3",
                        FaylFormati = "MP3", OrtachaBaho = 4.6, BaholarSoni = 43 },

                    new Kitob { Nomi = "Bolalar ertaklari to'plami — audio", NashrYili = 2023, SahifalarSoni = 0,
                        Nashriyot = "Kids Audio UZ", ISBN = "AUDIO-TEST-005", Til = "uz",
                        Narxi = 20000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "3-10 yoshli bolalar uchun o'zbek xalq ertaklari audio to'plami.",
                        Annotatsiya = "30 ta ertak, 6 soat. Ifodali o'qish.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739186-M.jpg",
                        MediaTuri = Kitob.MediaType.AudioKitob,
                        Elektron = true, ElektronManzil = "https://example.com/audio/kids-stories.mp3",
                        FaylFormati = "MP3", OrtachaBaho = 4.9, BaholarSoni = 117 },
                };

                foreach (var k in yangiAudiolar)
                {
                    if (await ctx.Kitoblar.AnyAsync(x => x.ISBN == k.ISBN)) continue;
                    ctx.Kitoblar.Add(k);
                    await ctx.SaveChangesAsync();
                    if (defaultKat != null)
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = k.Id, KategoriyaId = defaultKat.Id });
                    ctx.KitobNusxalari.Add(new KitobNusxa
                    {
                        KitobId = k.Id,
                        InventarRaqami = $"AUD-T{k.Id:D4}-01",
                        BarKod = $"AUD{k.Id:D6}01",
                        Holati = "Mavjud", JismoniyHolati = "Yangi",
                        QabulQilinganSana = DateTime.Now.AddMonths(-1),
                        KelibChiqishi = "Raqamli material",
                        FaylManzili = k.ElektronManzil, FaylFormati = k.FaylFormati,
                        YuklabOlishMumkin = true, Narxi = k.Narxi,
                        SotuvgaQoyilgan = true
                    });
                    await ctx.SaveChangesAsync();
                }
            }

            // ===== VIDEO KURSLAR =====
            int videoCount = await ctx.Kitoblar.CountAsync(k => k.MediaTuri == Kitob.MediaType.Video);
            if (videoCount < MIN)
            {
                var yangiVideolar = new[]
                {
                    new Kitob { Nomi = "JavaScript — noldan professional", NashrYili = 2024, SahifalarSoni = 0,
                        Nashriyot = "JS Academy UZ", ISBN = "VIDEO-TEST-001", Til = "uz",
                        Narxi = 120000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "JavaScript dasturlash tilini noldan professional darajagacha o'rgatuvchi video kurs.",
                        Annotatsiya = "80 ta dars, 32 soat video. ES6+, DOM, Fetch API, Node.js kirish.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739189-M.jpg",
                        MediaTuri = Kitob.MediaType.Video,
                        Elektron = true, ElektronManzil = "https://example.com/video/javascript-course",
                        FaylFormati = "MP4", OrtachaBaho = 4.9, BaholarSoni = 182 },

                    new Kitob { Nomi = "Grafik dizayn — Figma va Photoshop", NashrYili = 2023, SahifalarSoni = 0,
                        Nashriyot = "Design UZ", ISBN = "VIDEO-TEST-002", Til = "uz",
                        Narxi = 95000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Grafik dizayn asoslarini Figma va Photoshop orqali o'rgatuvchi kurs.",
                        Annotatsiya = "60 ta dars, 25 soat. UI/UX, branding, portfolio yaratish.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739190-M.jpg",
                        MediaTuri = Kitob.MediaType.Video,
                        Elektron = true, ElektronManzil = "https://example.com/video/graphic-design",
                        FaylFormati = "MP4", OrtachaBaho = 4.7, BaholarSoni = 134 },

                    new Kitob { Nomi = "Excel — biznes uchun", NashrYili = 2023, SahifalarSoni = 0,
                        Nashriyot = "Office Skills UZ", ISBN = "VIDEO-TEST-003", Til = "uz",
                        Narxi = 60000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Microsoft Excel dasturini biznes va tahlil uchun qo'llash.",
                        Annotatsiya = "45 ta dars, 18 soat. Formulalar, pivot table, makrolar.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739191-M.jpg",
                        MediaTuri = Kitob.MediaType.Video,
                        Elektron = true, ElektronManzil = "https://example.com/video/excel-business",
                        FaylFormati = "MP4", OrtachaBaho = 4.6, BaholarSoni = 97 },

                    new Kitob { Nomi = "Musiqa asoslari — gitara", NashrYili = 2022, SahifalarSoni = 0,
                        Nashriyot = "Music Academy UZ", ISBN = "VIDEO-TEST-004", Til = "uz",
                        Narxi = 80000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Gitara chalishni noldan o'rgatuvchi to'liq video kurs.",
                        Annotatsiya = "55 ta dars, 22 soat. Asosiy akkordlar, notalar, qo'shiqlar.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739192-M.jpg",
                        MediaTuri = Kitob.MediaType.Video,
                        Elektron = true, ElektronManzil = "https://example.com/video/guitar-basics",
                        FaylFormati = "MP4", OrtachaBaho = 4.8, BaholarSoni = 78 },

                    new Kitob { Nomi = "Marketing va SMM — 2024", NashrYili = 2024, SahifalarSoni = 0,
                        Nashriyot = "Marketing Hub UZ", ISBN = "VIDEO-TEST-005", Til = "uz",
                        Narxi = 110000, NusxalarSoni = 999, MavjudNusxalar = 999,
                        Tavsif = "Zamonaviy marketing va ijtimoiy tarmoqlar menejmentini o'rgatuvchi video kurs.",
                        Annotatsiya = "70 ta dars, 28 soat. Instagram, Telegram, Facebook, TikTok.",
                        Rasm = "https://covers.openlibrary.org/b/id/8739193-M.jpg",
                        MediaTuri = Kitob.MediaType.Video,
                        Elektron = true, ElektronManzil = "https://example.com/video/marketing-smm",
                        FaylFormati = "MP4", OrtachaBaho = 4.8, BaholarSoni = 156 },
                };

                foreach (var k in yangiVideolar)
                {
                    if (await ctx.Kitoblar.AnyAsync(x => x.ISBN == k.ISBN)) continue;
                    ctx.Kitoblar.Add(k);
                    await ctx.SaveChangesAsync();
                    if (defaultKat != null)
                        ctx.KitobKategoriyalari.Add(new KitobKategoriya { KitobId = k.Id, KategoriyaId = defaultKat.Id });
                    ctx.KitobNusxalari.Add(new KitobNusxa
                    {
                        KitobId = k.Id,
                        InventarRaqami = $"VID-T{k.Id:D4}-01",
                        BarKod = $"VID{k.Id:D6}01",
                        Holati = "Mavjud", JismoniyHolati = "Yangi",
                        QabulQilinganSana = DateTime.Now.AddMonths(-1),
                        KelibChiqishi = "Raqamli material",
                        FaylManzili = k.ElektronManzil, FaylFormati = k.FaylFormati,
                        YuklabOlishMumkin = true, Narxi = k.Narxi,
                        SotuvgaQoyilgan = true
                    });
                    await ctx.SaveChangesAsync();
                }
            }
        }

        // ==================== SOTUVGA QO'YILGAN NUSXALAR ====================
        /// <summary>
        /// Narxi > 0 bo'lgan jismoniy kitoblarning birinchi mavjud nusxasini sotuvga qo'yadi.
        /// Har applikatsiya ishga tushganda tekshiriladi (seed guard yo'q).
        /// </summary>
        private static async Task SeedSotuvgaQoyilganAsync(ApplicationDbContext ctx)
        {
            // Allaqachon kamida bitta sotuvda belgilangan nusxa bo'lsa, qaytamiz
            if (await ctx.KitobNusxalari.AnyAsync(kn => kn.SotuvgaQoyilgan))
                return;

            // Narxi > 0 bo'lgan kitoblar ro'yxatini olamiz
            var kitobIds = await ctx.Kitoblar
                .Where(k => k.Narxi != null && k.Narxi > 0)
                .Select(k => k.Id)
                .ToListAsync();

            int changed = 0;
            foreach (var kitobId in kitobIds)
            {
                // Har bir kitobdan birinchi mavjud nusxani olamiz (EF Core–xavfsiz loop)
                var nusxa = await ctx.KitobNusxalari
                    .Where(kn => kn.KitobId == kitobId && kn.Holati == "Mavjud")
                    .OrderBy(kn => kn.Id)
                    .FirstOrDefaultAsync();

                if (nusxa != null && !nusxa.SotuvgaQoyilgan)
                {
                    nusxa.SotuvgaQoyilgan = true;
                    changed++;
                }
            }

            if (changed > 0)
                await ctx.SaveChangesAsync();
        }

        // Mavjud kitoblarga rasm qo'shish
        private static async Task RasmlarniYangilashAsync(ApplicationDbContext ctx)
        {
            var rasmlar = new Dictionary<string, string>
            {
                ["O'tkan kunlar"] = "https://covers.openlibrary.org/b/id/10918-M.jpg",
                ["Mehrobdan chayon"] = "https://covers.openlibrary.org/b/id/10919-M.jpg",
                ["Kecha va kunduz"] = "https://covers.openlibrary.org/b/id/10920-M.jpg",
                ["Pul ishlashning boy otasi kambag'al otasidan farqlari"] = "https://covers.openlibrary.org/b/isbn/0743487451-M.jpg",
                ["Boy ota, kambag'al ota"] = "https://covers.openlibrary.org/b/isbn/0743487451-M.jpg",
                ["O'ylang va boyib bo'ling"] = "https://covers.openlibrary.org/b/isbn/1585424331-M.jpg",
                ["7 ta odatlar"] = "https://covers.openlibrary.org/b/isbn/0743269519-M.jpg",
                ["Urush va tinchlik"] = "https://covers.openlibrary.org/b/isbn/0140441867-M.jpg",
                ["Jinoyat va jazo"] = "https://covers.openlibrary.org/b/isbn/0140449136-M.jpg",
                ["Python dasturlash tili"] = "https://covers.openlibrary.org/b/isbn/1593279280-M.jpg",
                ["O'zbek tilining izohli lug'ati"] = "https://covers.openlibrary.org/b/id/8228-M.jpg",
                ["Algebra va matematik analiz"] = "https://covers.openlibrary.org/b/id/8229-M.jpg",
                ["Temur tuzuklari"] = "https://covers.openlibrary.org/b/id/8227-M.jpg",
                ["Toza kod"] = "https://covers.openlibrary.org/b/isbn/0132350882-M.jpg",
                ["Sapiens: Insoniyat tarixi"] = "https://covers.openlibrary.org/b/isbn/0062316095-M.jpg",
                ["Alkimyogar"] = "https://covers.openlibrary.org/b/isbn/0061122416-M.jpg",
                ["1984"] = "https://covers.openlibrary.org/b/isbn/0451524934-M.jpg",
                ["Kichkina shahzoda"] = "https://covers.openlibrary.org/b/isbn/0156012197-M.jpg",
                ["Sherlock Holmes sarguzashtlari"] = "https://covers.openlibrary.org/b/isbn/0141040432-M.jpg",
            };

            var kitoblar = await ctx.Kitoblar.ToListAsync();
            bool ozgardi = false;
            foreach (var kitob in kitoblar)
            {
                if (string.IsNullOrEmpty(kitob.Rasm) && rasmlar.TryGetValue(kitob.Nomi, out var rasm))
                {
                    kitob.Rasm = rasm;
                    ozgardi = true;
                }
            }
            if (ozgardi) await ctx.SaveChangesAsync();
        }

        // ==================== ABONEMENT TURLARI ====================
        private static async Task SeedAbonementTurlariAsync(ApplicationDbContext ctx)
        {
            if (await ctx.AbonementTurlari.AnyAsync()) return;
            ctx.AbonementTurlari.AddRange(
                new AbonementTuri { Nomi = "Oylik Standart", DavomiylikKun = 30, Narxi = 25000,
                    Tavsif = "Har oy 5 ta kitob bepul", BepulKitoblarSoni = 5, ChegirimFoizi = 0,
                    MaksimalKitoblar = 3, BepulYetkazish = false, Faol = true },
                new AbonementTuri { Nomi = "Choraklik Plus", DavomiylikKun = 90, Narxi = 65000,
                    Tavsif = "3 oy, 8 ta kitob/oy, 10% chegirma", BepulKitoblarSoni = 8, ChegirimFoizi = 10,
                    MaksimalKitoblar = 5, BepulYetkazish = false, Faol = true },
                new AbonementTuri { Nomi = "Yillik Premium", DavomiylikKun = 365, Narxi = 199000,
                    Tavsif = "Yil davomida cheksiz imkoniyatlar + bepul yetkazish",
                    BepulKitoblarSoni = 15, ChegirimFoizi = 20, MaksimalKitoblar = 7,
                    BepulYetkazish = true, Faol = true }
            );
            await ctx.SaveChangesAsync();
        }

        // ==================== ABONEMENTLAR ====================
        private static async Task SeedAbonementlarAsync(ApplicationDbContext ctx, List<ApplicationUser> users)
        {
            if (await ctx.Abonementlar.AnyAsync() || !users.Any()) return;
            var turlar = await ctx.AbonementTurlari.ToListAsync();
            if (!turlar.Any()) return;

            for (int i = 0; i < Math.Min(4, users.Count); i++)
            {
                var tur = turlar[i % turlar.Count];
                ctx.Abonementlar.Add(new Abonement
                {
                    FoydalanuvchiId = users[i].Id,
                    AbonementTuriId = tur.Id,
                    BoshlanishSana = DateTime.Now.AddDays(-Random.Shared.Next(10, 60)),
                    TugashSana = DateTime.Now.AddDays(tur.DavomiylikKun - Random.Shared.Next(10, 30)),
                    TolanganNarx = tur.Narxi, Holati = "Faol",
                    TolovUsuli = i % 2 == 0 ? "Click" : "Payme"
                });
            }
            await ctx.SaveChangesAsync();
        }

        // ==================== BRONLAR VA BERUVLAR ====================
        private static async Task SeedBronlarVaBeruvlarAsync(ApplicationDbContext ctx, List<ApplicationUser> users)
        {
            if (await ctx.Bronlar.AnyAsync() || !users.Any()) return;

            var berilganNusxalar = await ctx.KitobNusxalari
                .Include(n => n.Kitob).Where(n => n.Holati == "Berilgan").Take(5).ToListAsync();

            for (int i = 0; i < Math.Min(berilganNusxalar.Count, users.Count); i++)
            {
                var nusxa = berilganNusxalar[i];
                var user = users[i % users.Count];

                ctx.Bronlar.Add(new Bron
                {
                    FoydalanuvchiId = user.Id, KitobId = nusxa.KitobId, KitobNusxaId = nusxa.Id,
                    BronSana = DateTime.Now.AddDays(-Random.Shared.Next(5, 30)),
                    AmalMuddati = DateTime.Now.AddDays(3), Holati = "Bajarilgan"
                });
                ctx.Beruvlar.Add(new Beruv
                {
                    FoydalanuvchiId = user.Id, KitobNusxaId = nusxa.Id,
                    BeruvRaqami = $"BRV{DateTime.Now.Year}{i + 1:D4}",
                    BerilganSana = DateTime.Now.AddDays(-Random.Shared.Next(5, 25)),
                    QaytarishMuddati = DateTime.Now.AddDays(Random.Shared.Next(5, 25)),
                    Holati = "Berilgan", BerganXodim = "Tizim"
                });
            }

            // Muddati o'tgan va jarimali
            var mavjudNusxalar = await ctx.KitobNusxalari.Where(n => n.Holati == "Mavjud").Take(2).ToListAsync();
            for (int i = 0; i < Math.Min(2, Math.Min(mavjudNusxalar.Count, users.Count)); i++)
            {
                ctx.Beruvlar.Add(new Beruv
                {
                    FoydalanuvchiId = users[i].Id, KitobNusxaId = mavjudNusxalar[i].Id,
                    BeruvRaqami = $"OVR{i + 1:D4}",
                    BerilganSana = DateTime.Now.AddDays(-45),
                    QaytarishMuddati = DateTime.Now.AddDays(-15),
                    QaytarilganSana = DateTime.Now.AddDays(-5),
                    Holati = "Qaytarilgan", BerganXodim = "Tizim"
                });
                ctx.Jarimalar.Add(new Jarima
                {
                    FoydalanuvchiId = users[i].Id, BeruvId = 0, Miqdor = 10000,
                    Valyuta = "UZS", JarimaTuri = "Kechiktirilgan",
                    Sabab = "Kitob 10 kun kechiktirilgan", BerilganSana = DateTime.Now.AddDays(-5),
                    Holati = i == 0 ? "Kutilmoqda" : "To'langan",
                    TolanganSana = i == 0 ? null : DateTime.Now.AddDays(-3),
                    TolovUsuli = i == 0 ? null : "Naqd"
                });
            }
            await ctx.SaveChangesAsync();
        }

        // ==================== REYTINGLAR ====================
        private static async Task SeedReytinglarAsync(ApplicationDbContext ctx, List<ApplicationUser> users)
        {
            if (await ctx.Reytinglar.AnyAsync() || !users.Any()) return;
            var kitoblar = await ctx.Kitoblar.Take(12).ToListAsync();
            var sharhlar = new[] {
                "Juda zo'r kitob, tavsiya qilaman!",
                "Hayotimni o'zgartirgan asar.",
                "Har bir sahifasi qiziqarli va ta'sirli.",
                "O'qish kerak bo'lgan kitoblardan biri.",
                "Yangicha qarash beradi odamga.",
                "Juda yaxshi yozilgan, tushunish oson.",
                "Klassika — har doim dolzarb.",
            };
            foreach (var kitob in kitoblar)
                foreach (var user in users.Take(4))
                    if (!await ctx.Reytinglar.AnyAsync(r => r.FoydalanuvchiId == user.Id && r.KitobId == kitob.Id))
                        ctx.Reytinglar.Add(new Reyting
                        {
                            FoydalanuvchiId = user.Id, KitobId = kitob.Id,
                            Baho = Random.Shared.Next(4, 6),
                            Sharh = sharhlar[Random.Shared.Next(sharhlar.Length)],
                            YaratilganSana = DateTime.Now.AddDays(-Random.Shared.Next(1, 90))
                        });
            await ctx.SaveChangesAsync();
        }

        // ==================== BILDIRISHNOMALAR ====================
        private static async Task SeedBildirishnomalarAsync(ApplicationDbContext ctx, List<ApplicationUser> users)
        {
            if (await ctx.Bildirishnomalar.AnyAsync() || !users.Any()) return;
            foreach (var user in users.Take(5))
            {
                ctx.Bildirishnomalar.Add(new Bildirishnoma
                {
                    FoydalanuvchiId = user.Id,
                    Sarlavha = "Kutubxona tizimiga xush kelibsiz! 👋",
                    Matn = $"Hurmatli {user.Ism}, KutubxonaAT tizimiga xush kelibsiz! Kitoblarni bron qiling, ijara oling.",
                    Tur = "Umumiy", YuborilganSana = DateTime.Now.AddDays(-5), Oqilgan = false
                });
                ctx.Bildirishnomalar.Add(new Bildirishnoma
                {
                    FoydalanuvchiId = user.Id,
                    Sarlavha = "📚 Yangi kitoblar va video kurslar qo'shildi!",
                    Matn = "Audio kitoblar, elektron nashrlar va video darsliklar katalogga qo'shildi. Ko'rib chiqing!",
                    Tur = "Yangilik", Havola = "/User/Book/Index",
                    YuborilganSana = DateTime.Now.AddDays(-1), Oqilgan = false
                });
            }
            await ctx.SaveChangesAsync();
        }
    }
}
