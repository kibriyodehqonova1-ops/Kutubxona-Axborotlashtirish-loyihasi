using KutubxonaAt.Data;
using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Beruv
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;   // int → string

        [Required]
        public int KitobNusxaId { get; set; }

        public string? BeruvRaqami { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        public DateTime BerilganSana { get; set; } = DateTime.Now;
        public DateTime QaytarishMuddati { get; set; } = DateTime.Now.AddDays(30);
        public DateTime? QaytarilganSana { get; set; }

        public string Holati { get; set; } = "Berilgan"; // Berilgan, Qaytarilgan, MuddatiO‘tgan, Uzaytirilgan

        public int UzaytirishlarSoni { get; set; } = 0;
        public DateTime? UzaytirilganMuddat { get; set; }

        public string? BerganXodim { get; set; }
        public string? QabulQilganXodim { get; set; }

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual KitobNusxa? KitobNusxa { get; set; }
        public virtual ICollection<Jarima> Jarimalar { get; set; } = new List<Jarima>();
    }
}