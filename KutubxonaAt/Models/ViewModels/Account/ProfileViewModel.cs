using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Ism majburiy")]
        [StringLength(50)]
        [Display(Name = "Ism")]
        public string Ism { get; set; } = string.Empty;

        [Required(ErrorMessage = "Familiya majburiy")]
        [StringLength(50)]
        [Display(Name = "Familiya")]
        public string Familiya { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Otasining ismi")]
        public string? Sharif { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty; // O'qish uchun, tahrirlanmaydi

        [Phone]
        [Display(Name = "Telefon raqami")]
        public string? Telefon { get; set; }

        [StringLength(200)]
        [Display(Name = "Manzil")]
        public string? Manzil { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Tug'ilgan sana")]
        public DateTime? TugilganSana { get; set; }

        [Display(Name = "Foydalanuvchi turi")]
        public string Turi { get; set; } = string.Empty;

        [Display(Name = "Fakultet")]
        public string? Fakultet { get; set; }

        [Display(Name = "Guruh")]
        public string? Guruh { get; set; }

        [Display(Name = "Talaba ID")]
        public string? TalabaId { get; set; }

        [Display(Name = "Rasm")]
        public string? Rasm { get; set; }

        [Display(Name = "Izoh")]
        public string? Izoh { get; set; }

        [Display(Name = "Ro'yxatdan o'tgan sana")]
        public DateTime RoyxatSanasi { get; set; }

        [Display(Name = "Amal qilish muddati")]
        public DateTime? AmalMuddati { get; set; }

        [Display(Name = "Faol")]
        public bool Faol { get; set; }
    }
}   