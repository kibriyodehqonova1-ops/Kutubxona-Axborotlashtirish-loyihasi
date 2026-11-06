using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutubxonaAt.Models
{
    public class KitobNusxa
    {
        public int Id { get; set; }

        [Required]
        public int KitobId { get; set; }

        // Jismoniy nusxa uchun majburiy, raqamli nusxalar uchun ixtiyoriy (kelajakda)
        [Required(ErrorMessage = "Inventar raqami majburiy")]
        [StringLength(50)]
        public string InventarRaqami { get; set; } = string.Empty;

        [StringLength(100)]
        public string? BarKod { get; set; }

        [StringLength(100)]
        public string? RFID { get; set; }
        // ... mavjud maydonlar ...
        public bool SotuvgaQoyilgan { get; set; } = false;

        public DateTime QabulQilinganSana { get; set; } = DateTime.Now;
        public string? KelibChiqishi { get; set; } // Sotib olingan, Sovg‘a, Almashuv

        [DataType(DataType.Currency)]
        public decimal? Narxi { get; set; }

        // Nusxa holati (faqat jismoniy nusxalar uchun)
        public string Holati { get; set; } = "Mavjud"; // Mavjud, Berilgan, Ta'mirda, Yo'qolgan

        // Jismoniy holati (faqat jismoniy nusxalar uchun)
        public string JismoniyHolati { get; set; } = "Yaxshi"; // Yangi, Yaxshi, Eskirgan, Shikastlangan

        [StringLength(100)]
        public string? Joylashuv { get; set; } // Qaysi zal, javon

        public bool OquvZalidanChiqmaydi { get; set; } = false;
        public bool Cheklangan { get; set; } = false;
        public DateTime? OxirgiInventarizatsiya { get; set; }

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Raqamli nusxa maydonlari (kelajakda raqamli resurslarni nusxa darajasida boshqarish uchun)
        public string? FaylManzili { get; set; }      // Yuklangan fayl manzili
        public string? FaylFormati { get; set; }      // PDF, MP3, EPUB, MP4, ...
        public long? FaylHajmi { get; set; }          // Baytlarda
        public bool YuklabOlishMumkin { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(KitobId))]
        public virtual Kitob? Kitob { get; set; }
        public virtual ICollection<Beruv> Beruvlar { get; set; } = new List<Beruv>();
    }
}