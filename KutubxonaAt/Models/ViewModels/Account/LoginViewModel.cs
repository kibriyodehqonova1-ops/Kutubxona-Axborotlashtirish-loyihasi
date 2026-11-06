using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parol majburiy")]
        [DataType(DataType.Password)]
        [Display(Name = "Parol")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Meni eslab qol")]
        public bool RememberMe { get; set; }
    }
}