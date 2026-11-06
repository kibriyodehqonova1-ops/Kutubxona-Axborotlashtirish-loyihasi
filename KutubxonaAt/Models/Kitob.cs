using KutubxonaAt.Models;
using   KutubxonaAt.Data;
using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Kitob
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitob nomi majburiy")]
        [StringLength(500)]
        public string Nomi { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AslNomi { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        [StringLength(1000)]
        public string? Tavsif { get; set; }

        [StringLength(1000)]
        public string? Annotatsiya { get; set; }

        public int? NashrYili { get; set; }
        public int? SahifalarSoni { get; set; }
        public string? Nashriyot { get; set; }
        public string? Shahar { get; set; }

        [StringLength(50)]
        public string? Til { get; set; } = "uz";

        [StringLength(50)]
        public string? UDK { get; set; }

        [StringLength(50)]
        public string? BBK { get; set; }

        [StringLength(50)]
        public string? MuallifBelgisi { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Narxi { get; set; }

        public string? Rasm { get; set; }

        public DateTime QoshilganSana { get; set; } = DateTime.Now;
        public DateTime? TahrirlanganSana { get; set; }

        public int NusxalarSoni { get; set; } = 1;
        public int MavjudNusxalar { get; set; } = 1;

        public bool Elektron { get; set; } = false;
        public string? ElektronManzil { get; set; }
        public byte[]? ElektronFayl { get; set; }
        public string? FaylFormati { get; set; } // PDF, EPUB

        public int KorishlarSoni { get; set; } = 0;
        public double OrtachaBaho { get; set; } = 0;
        public int BaholarSoni { get; set; } = 0;
        public MediaType MediaTuri { get; set; } = MediaType.Kitob;
        public enum MediaType
        {
            Kitob = 0,
            ElektronKitob = 1,
            AudioKitob = 2,
            Jurnal = 3,
            Video = 4
        }

        // Navigation properties
        public virtual ICollection<KitobMuallif> KitobMualliflari { get; set; } = new List<KitobMuallif>();
        public virtual ICollection<KitobKategoriya> KitobKategoriyalari { get; set; } = new List<KitobKategoriya>();
        public virtual ICollection<KitobNusxa> KitobNusxalari { get; set; } = new List<KitobNusxa>();
        public virtual ICollection<Reyting> Reytinglar { get; set; } = new List<Reyting>();
        public virtual ICollection<Bron> Bronlar { get; set; } = new List<Bron>();
        public virtual ICollection<KutishRoyxati> KutishRoyxati { get; set; } = new List<KutishRoyxati>();
    }
}