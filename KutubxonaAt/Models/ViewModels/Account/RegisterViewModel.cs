using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.Account
{
    public class RegisterViewModel
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

        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Telefon raqami")]
        public string? Telefon { get; set; }

        [Display(Name = "Manzil")]
        public string? Manzil { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Tug'ilgan sana")]
        public DateTime? TugilganSana { get; set; }

        [Display(Name = "Foydalanuvchi turi")]
        public string? Turi { get; set; } = "Boshqa";

        [Display(Name = "Fakultet")]
        public string? Fakultet { get; set; }

        [Display(Name = "Guruh")]
        public string? Guruh { get; set; }

        [Display(Name = "Talaba ID")]
        public string? TalabaId { get; set; }

        [Required(ErrorMessage = "Parol majburiy")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parol kamida 6 belgidan iborat bo‘lishi kerak.")]
        [Display(Name = "Parol")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Parolni tasdiqlang")]
        [Compare("Password", ErrorMessage = "Parol va tasdiqlash mos kelmadi.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}