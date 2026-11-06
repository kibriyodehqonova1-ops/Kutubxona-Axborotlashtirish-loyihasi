using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.User
{
    public class UserEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ism majburiy")]
        [StringLength(50)]
        public string Ism { get; set; } = string.Empty;

        [Required(ErrorMessage = "Familiya majburiy")]
        [StringLength(50)]
        public string Familiya { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Sharif { get; set; }

        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Telefon { get; set; }

        public string? Manzil { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TugilganSana { get; set; }

        [Required]
        public string Turi { get; set; } = "Talaba";

        public string? Fakultet { get; set; }
        public string? Guruh { get; set; }
        public string? TalabaId { get; set; }

        public bool Faol { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime? AmalMuddati { get; set; }

        // Rollar
        public List<string> SelectedRoles { get; set; } = new List<string>();
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
    }
}