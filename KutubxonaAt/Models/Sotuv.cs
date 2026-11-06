using KutubxonaAt.Models;
using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    public class Sotuv
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        [Required]
        public int KitobId { get; set; }

        public DateTime SotibOlinganSana { get; set; } = DateTime.Now;

        [DataType(DataType.Currency)]
        public decimal TolanganNarx { get; set; }

        public string? TolovUsuli { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual Kitob? Kitob { get; set; }
    }
}