using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Kategoriya
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategoriya nomi majburiy")]
        [StringLength(100)]
        public string Nomi { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Tavsif { get; set; }

        public int? OtaKategoriyaId { get; set; }

        // Navigation
        public virtual Kategoriya? OtaKategoriya { get; set; }
        public virtual ICollection<Kategoriya> IchkiKategoriyalar { get; set; } = new List<Kategoriya>();
        public virtual ICollection<KitobKategoriya> KitobKategoriyalari { get; set; } = new List<KitobKategoriya>();
    }
}