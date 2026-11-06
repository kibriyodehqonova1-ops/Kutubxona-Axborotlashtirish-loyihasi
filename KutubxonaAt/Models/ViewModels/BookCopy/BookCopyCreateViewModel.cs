using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.BookCopy
{
    public class BookCopyCreateViewModel
    {
        public int KitobId { get; set; }
        public string? KitobNomi { get; set; }

        [Required(ErrorMessage = "Inventar raqami majburiy")]
        [StringLength(50)]
        public string InventarRaqami { get; set; } = string.Empty;

        [StringLength(100)]
        public string? BarKod { get; set; }

        [StringLength(100)]
        public string? RFID { get; set; }

        [DataType(DataType.Date)]
        public DateTime QabulQilinganSana { get; set; } = DateTime.Now;

        public string? KelibChiqishi { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Narxi { get; set; }

        public string Holati { get; set; } = "Mavjud";
        public string JismoniyHolati { get; set; } = "Yaxshi";

        [StringLength(100)]
        public string? Joylashuv { get; set; }

        public bool OquvZalidanChiqmaydi { get; set; } = false;
        public bool Cheklangan { get; set; } = false;

        [DataType(DataType.Date)]
        public DateTime? OxirgiInventarizatsiya { get; set; }

        [StringLength(500)]
        public string? Izoh { get; set; }
    }
}