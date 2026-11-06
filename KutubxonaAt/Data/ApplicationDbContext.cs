using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KutubxonaAt.Models;

namespace KutubxonaAt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet'lar
        public DbSet<Kitob> Kitoblar { get; set; }
        public DbSet<KitobNusxa> KitobNusxalari { get; set; }
        public DbSet<Muallif> Mualliflar { get; set; }
        public DbSet<Kategoriya> Kategoriyalar { get; set; }
        public DbSet<KitobKategoriya> KitobKategoriyalari { get; set; }
        public DbSet<KitobMuallif> KitobMualliflari { get; set; }
        public DbSet<Beruv> Beruvlar { get; set; }
        public DbSet<Bron> Bronlar { get; set; }
        public DbSet<Jarima> Jarimalar { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Bildirishnoma> Bildirishnomalar { get; set; }
        public DbSet<Reyting> Reytinglar { get; set; }
        public DbSet<FaoliyatLog> FaoliyatLoglar { get; set; }
        public DbSet<Sotuv> Sotuvlar { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Yangi funksiyalar
        public DbSet<KutishRoyxati> KutishRoyxati { get; set; }
        public DbSet<AbonementTuri> AbonementTurlari { get; set; }
        public DbSet<Abonement> Abonementlar { get; set; }
        public DbSet<YetkazibBerish> YetkazibBerishlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many: Kitob <-> Kategoriya
            modelBuilder.Entity<KitobKategoriya>(entity =>
            {
                entity.HasKey(kk => new { kk.KitobId, kk.KategoriyaId });
                entity.HasOne(kk => kk.Kitob)
                    .WithMany(k => k.KitobKategoriyalari)
                    .HasForeignKey(kk => kk.KitobId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(kk => kk.Kategoriya)
                    .WithMany(k => k.KitobKategoriyalari)
                    .HasForeignKey(kk => kk.KategoriyaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Many-to-many: Kitob <-> Muallif
            modelBuilder.Entity<KitobMuallif>(entity =>
            {
                entity.HasKey(km => new { km.KitobId, km.MuallifId });
                entity.HasOne(km => km.Kitob)
                    .WithMany(k => k.KitobMualliflari)
                    .HasForeignKey(km => km.KitobId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(km => km.Muallif)
                    .WithMany(m => m.KitobMualliflari)
                    .HasForeignKey(km => km.MuallifId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------- Beruv --------------------
            modelBuilder.Entity<Beruv>()
                .HasOne(b => b.Foydalanuvchi)
                .WithMany(f => f.Beruvlar)
                .HasForeignKey(b => b.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Beruv>()
                .HasOne(b => b.KitobNusxa)
                .WithMany(kn => kn.Beruvlar)
                .HasForeignKey(b => b.KitobNusxaId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------- Bron --------------------
            modelBuilder.Entity<Bron>()
                .HasOne(b => b.Foydalanuvchi)
                .WithMany(f => f.Bronlar)
                .HasForeignKey(b => b.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bron>()
                .HasOne(b => b.Kitob)
                .WithMany(k => k.Bronlar)
                .HasForeignKey(b => b.KitobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bron>()
                .HasOne(b => b.KitobNusxa)
                .WithMany()
                .HasForeignKey(b => b.KitobNusxaId)
                .OnDelete(DeleteBehavior.SetNull);

            // -------------------- Jarima --------------------
            modelBuilder.Entity<Jarima>()
                .HasOne(j => j.Foydalanuvchi)
                .WithMany(f => f.Jarimalar)
                .HasForeignKey(j => j.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jarima>()
                .HasOne(j => j.Beruv)
                .WithMany(b => b.Jarimalar)
                .HasForeignKey(j => j.BeruvId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------- Reyting --------------------
            modelBuilder.Entity<Reyting>()
                .HasOne(r => r.Foydalanuvchi)
                .WithMany(u => u.Reytinglar)
                .HasForeignKey(r => r.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reyting>()
                .HasOne(r => r.Kitob)
                .WithMany(k => k.Reytinglar)
                .HasForeignKey(r => r.KitobId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------- Bildirishnoma --------------------
            modelBuilder.Entity<Bildirishnoma>()
                .HasOne(b => b.Foydalanuvchi)
                .WithMany(u => u.Bildirishnomalar)
                .HasForeignKey(b => b.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------- FaoliyatLog --------------------
            modelBuilder.Entity<FaoliyatLog>()
                .HasOne(f => f.Foydalanuvchi)
                .WithMany(u => u.FaoliyatLoglari)
                .HasForeignKey(f => f.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.SetNull);

            // -------------------- OrderItem --------------------
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Kitob)
                .WithMany()
                .HasForeignKey(oi => oi.KitobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.KitobNusxa)
                .WithMany()
                .HasForeignKey(oi => oi.KitobNusxaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------- KutishRoyxati --------------------
            modelBuilder.Entity<KutishRoyxati>()
                .HasOne(k => k.Foydalanuvchi)
                .WithMany(u => u.KutishRoyxati)
                .HasForeignKey(k => k.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KutishRoyxati>()
                .HasOne(k => k.Kitob)
                .WithMany(k => k.KutishRoyxati)
                .HasForeignKey(k => k.KitobId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------- Abonement --------------------
            modelBuilder.Entity<Abonement>()
                .HasOne(a => a.Foydalanuvchi)
                .WithMany(u => u.Abonementlar)
                .HasForeignKey(a => a.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Abonement>()
                .HasOne(a => a.AbonementTuri)
                .WithMany(t => t.Abonementlar)
                .HasForeignKey(a => a.AbonementTuriId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------- YetkazibBerish --------------------
            modelBuilder.Entity<YetkazibBerish>()
                .HasOne(y => y.Foydalanuvchi)
                .WithMany(u => u.YetkazibBerishlar)
                .HasForeignKey(y => y.FoydalanuvchiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<YetkazibBerish>()
                .HasOne(y => y.Beruv)
                .WithMany()
                .HasForeignKey(y => y.BeruvId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<YetkazibBerish>()
                .HasOne(y => y.Bron)
                .WithMany()
                .HasForeignKey(y => y.BronId)
                .OnDelete(DeleteBehavior.SetNull);

            // ==================== Decimal aniqligi ====================
            modelBuilder.Entity<Jarima>()
                .Property(j => j.Miqdor)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.JamiSumma)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Narxi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Kitob>()
                .Property(k => k.Narxi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<KitobNusxa>()
                .Property(kn => kn.Narxi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sotuv>()
                .Property(s => s.TolanganNarx)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AbonementTuri>()
                .Property(a => a.Narxi)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AbonementTuri>()
                .Property(a => a.ChegirimFoizi)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Abonement>()
                .Property(a => a.TolanganNarx)
                .HasPrecision(18, 2);

            modelBuilder.Entity<YetkazibBerish>()
                .Property(y => y.YetkazishNarxi)
                .HasPrecision(18, 2);
        }
    }
}
