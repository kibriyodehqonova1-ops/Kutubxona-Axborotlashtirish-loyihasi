using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutubxonaAt.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        public DateTime Sana { get; set; } = DateTime.Now;

        [DataType(DataType.Currency)]
        public decimal JamiSumma { get; set; }

        public string Holati { get; set; } = "Kutilmoqda"; // Kutilmoqda, To‘langan, Bekor qilingan

        public string TolovUsuli { get; set; } = string.Empty; // Naqd, Click, Payme, Plastik karta

        public string? Manzil { get; set; }

        public string? Izoh { get; set; }

        // Navigation
        [ForeignKey(nameof(FoydalanuvchiId))]
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}