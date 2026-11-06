using KutubxonaAt.Models;
using Microsoft.AspNetCore.Identity;

namespace KutubxonaAt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Ism { get; set; } = string.Empty;
        public string Familiya { get; set; } = string.Empty;
        public string? Sharif { get; set; }
        public string? Telefon { get; set; }
        public string? Manzil { get; set; }
        public DateTime? TugilganSana { get; set; }
        public string Turi { get; set; } = "Talaba"; // Talaba, O'qituvchi, Xodim, Admin
        public string? Fakultet { get; set; }
        public string? Guruh { get; set; }
        public string? TalabaId { get; set; }
        public DateTime RoyxatSanasi { get; set; } = DateTime.Now;
        public DateTime? OxirgiKirish { get; set; }
        public DateTime? AmalMuddati { get; set; }
        public bool Faol { get; set; } = true;
        public string? Rasm { get; set; }
        public string? Izoh { get; set; }

        // Navigation properties
        public virtual ICollection<Beruv> Beruvlar { get; set; } = new List<Beruv>();
        public virtual ICollection<Bron> Bronlar { get; set; } = new List<Bron>();
        public virtual ICollection<Jarima> Jarimalar { get; set; } = new List<Jarima>();
        public virtual ICollection<Bildirishnoma> Bildirishnomalar { get; set; } = new List<Bildirishnoma>();
        public virtual ICollection<Reyting> Reytinglar { get; set; } = new List<Reyting>();
        public virtual ICollection<FaoliyatLog> FaoliyatLoglari { get; set; } = new List<FaoliyatLog>();
        public virtual ICollection<KutishRoyxati> KutishRoyxati { get; set; } = new List<KutishRoyxati>();
        public virtual ICollection<Abonement> Abonementlar { get; set; } = new List<Abonement>();
        public virtual ICollection<YetkazibBerish> YetkazibBerishlar { get; set; } = new List<YetkazibBerish>();
    }
}