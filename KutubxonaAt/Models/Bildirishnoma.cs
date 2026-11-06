using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Bildirishnoma
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Sarlavha { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Matn { get; set; } = string.Empty;

        public DateTime YuborilganSana { get; set; } = DateTime.Now;
        public bool Oqilgan { get; set; } = false;
        public DateTime? OqilganSana { get; set; }

        public string Tur { get; set; } = "Umumiy"; // Umumiy, Muddat, Jarima, Yangilik
        public string? Havola { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
    }
}