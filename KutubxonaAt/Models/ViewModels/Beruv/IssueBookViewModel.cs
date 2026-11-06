using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.ViewModels.Beruv
{
    public class IssueBookViewModel
    {
        [Required(ErrorMessage = "Foydalanuvchini tanlang")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kitob nusxasini tanlang")]
        public int KitobNusxaId { get; set; }

        [Required(ErrorMessage = "Beriladigan kunlar sonini kiriting")]
        [Range(1, 60, ErrorMessage = "1 dan 60 kungacha bo‘lishi mumkin")]
        public int LoanDays { get; set; } = 30;

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Select list'lar uchun
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> BookCopies { get; set; } = new();
    }
}