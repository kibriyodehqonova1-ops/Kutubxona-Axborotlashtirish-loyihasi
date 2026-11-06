using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class FaoliyatLog
    {
        public int Id { get; set; }

        public string? FoydalanuvchiId { get; set; }

        [Required]
        [StringLength(50)]
        public string AmalTuri { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Tavsif { get; set; } = string.Empty;

        [StringLength(45)]
        public string? IP { get; set; }

        [StringLength(500)]
        public string? Qurilma { get; set; }

        public DateTime Sana { get; set; } = DateTime.Now;

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
    }
}