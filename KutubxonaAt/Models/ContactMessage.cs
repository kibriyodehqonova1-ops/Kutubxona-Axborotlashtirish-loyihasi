using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // ApplicationUser.Id (string)

        [Required]
        [StringLength(100)]
        public string Ism { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Familiya { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Telefon { get; set; }

        [Required]
        [StringLength(2000)]
        public string Xabar { get; set; } = string.Empty;

        public DateTime YuborilganSana { get; set; } = DateTime.Now;

        public bool Oqilgan { get; set; } = false;   // Admin o‘qiganmi?
        public DateTime? OqilganSana { get; set; }

        public string? AdminJavobi { get; set; }      // Admin javobi
        public DateTime? JavobYozilganSana { get; set; }

        // Navigation (ixtiyoriy)
        public virtual ApplicationUser? User { get; set; }
    }
}