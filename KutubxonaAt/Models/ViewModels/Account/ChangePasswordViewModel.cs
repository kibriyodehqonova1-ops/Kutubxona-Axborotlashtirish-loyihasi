using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Eski parol majburiy")]
        [DataType(DataType.Password)]
        [Display(Name = "Eski parol")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yangi parol majburiy")]
        [StringLength(100, ErrorMessage = "{0} kamida {2} va eng ko‘pi {1} belgidan iborat bo‘lishi kerak.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yangi parol")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Yangi parolni tasdiqlang")]
        [Compare("NewPassword", ErrorMessage = "Yangi parol va tasdiqlash mos kelmadi.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}