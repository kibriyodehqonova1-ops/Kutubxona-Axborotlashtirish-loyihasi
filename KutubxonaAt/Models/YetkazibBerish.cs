using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    /// <summary>
    /// Kitobni manzilga yetkazib berish buyurtmasi
    /// </summary>
    public class YetkazibBerish
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        /// <summary>Beruv yoki Bron ID bilan bog'liq</summary>
        public int? BeruvId { get; set; }
        public int? BronId { get; set; }

        [Required(ErrorMessage = "Yetkazish manzili majburiy")]
        [StringLength(500)]
        public string Manzil { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Shahar { get; set; }

        [StringLength(20)]
        public string? TelefонRaqam { get; set; }

        public decimal YetkazishNarxi { get; set; } = 5000;

        public string Holati { get; set; } = "Tayyorlanmoqda";
        // Holatlari: Tayyorlanmoqda → Yo'lda → Yetkazildi | Bekor qilingan

        public DateTime BuyurtmaSana { get; set; } = DateTime.Now;
        public DateTime? YetkazilishKutilganSana { get; set; }
        public DateTime? YetkazilganSana { get; set; }

        /// <summary>Haydovchi ismi yoki ID</summary>
        [StringLength(200)]
        public string? HaydovchiMaʼlumoti { get; set; }

        [StringLength(100)]
        public string? KuzatuvRaqami { get; set; }

        [StringLength(500)]
        public string? Izoh { get; set; }

        public bool TolovQilinganmi { get; set; } = false;

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Beruv? Beruv { get; set; }
        public virtual Bron? Bron { get; set; }
    }
}
