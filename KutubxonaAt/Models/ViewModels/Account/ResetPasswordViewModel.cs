using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parol majburiy")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parol kamida 6 belgidan iborat bo'lishi kerak.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yangi parol")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Parolni tasdiqlang")]
        [Compare("Password", ErrorMessage = "Parol va tasdiqlash mos kelmadi.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
