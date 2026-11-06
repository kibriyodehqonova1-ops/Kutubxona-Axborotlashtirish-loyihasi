using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Muallif
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ism majburiy")]
        [StringLength(100)]
        public string Ism { get; set; } = string.Empty;

        [Required(ErrorMessage = "Familiya majburiy")]
        [StringLength(100)]
        public string Familiya { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Sharif { get; set; }

        [StringLength(100)]
        public string? Taxallus { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TugilganSana { get; set; }

        [DataType(DataType.Date)]
        public DateTime? VafotSana { get; set; }

        [StringLength(2000)]
        public string? Biografiya { get; set; }

        public string? Davlat { get; set; }
        public string? Rasm { get; set; }

        // Navigation
        public virtual ICollection<KitobMuallif> KitobMualliflari { get; set; } = new List<KitobMuallif>();
    }
}