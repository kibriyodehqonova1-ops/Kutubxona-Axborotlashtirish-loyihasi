using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Bron
    {

        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;   // int → string
        [Required]
        public int KitobId { get; set; }

        public DateTime BronSana { get; set; } = DateTime.Now;
        public DateTime AmalMuddati { get; set; } = DateTime.Now.AddDays(3);
        public string Holati { get; set; } = "Faol"; // Faol, Bekor qilingan, Bajarilgan

        public int? KitobNusxaId { get; set; } // Qaysi nusxa bron qilingan (ixtiyoriy)

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Kitob? Kitob { get; set; }
        public virtual KitobNusxa? KitobNusxa { get; set; }
    }
}