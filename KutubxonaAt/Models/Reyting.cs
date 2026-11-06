using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Reyting
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        [Required]
        public int KitobId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Baho { get; set; }

        [StringLength(1000)]
        public string? Sharh { get; set; }

        public DateTime YaratilganSana { get; set; } = DateTime.Now;
        public DateTime? TahrirlanganSana { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Kitob? Kitob { get; set; }
    }
}