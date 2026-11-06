using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Jarima
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;   // int → string
        public int? BeruvId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Miqdor { get; set; }

        public string Valyuta { get; set; } = "UZS";

        public string JarimaTuri { get; set; } = "Kechiktirilgan"; // Kechiktirilgan, Shikastlangan, Yo'qolgan

        [Required]
        [StringLength(500)]
        public string Sabab { get; set; } = string.Empty;

        public DateTime BerilganSana { get; set; } = DateTime.Now;
        public DateTime? TolanganSana { get; set; }
        public string Holati { get; set; } = "Kutilmoqda"; // Kutilmoqda, To'langan, Bekor qilingan

        public string? TolovUsuli { get; set; } // Naqd, Karta, Click, Payme
        public string? Izoh { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Beruv? Beruv { get; set; }
    }
}