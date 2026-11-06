using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    /// <summary>
    /// Kitob mavjud bo'lmaganda kutish ro'yxati
    /// </summary>
    public class KutishRoyxati
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        [Required]
        public int KitobId { get; set; }

        public DateTime QoshilganSana { get; set; } = DateTime.Now;

        public string Holati { get; set; } = "Kutmoqda"; // Kutmoqda, Bildirildi, Bekor qilingan, Bajarilgan

        public DateTime? BildirilganSana { get; set; }

        public int Tartib { get; set; } = 0; // Navbat raqami

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Kitob? Kitob { get; set; }
    }
}
