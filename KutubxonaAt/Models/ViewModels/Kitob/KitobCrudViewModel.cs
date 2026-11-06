using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static KutubxonaAt.Models.Kitob;

namespace KutubxonaAt.ViewModels.Kitob
{
    public class KitobCrudViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitob nomi majburiy")]
        [StringLength(500)]
        public string Nomi { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AslNomi { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        [StringLength(1000)]
        public string? Tavsif { get; set; }

        [StringLength(1000)]
        public string? Annotatsiya { get; set; }

        public int? NashrYili { get; set; }
        public int? SahifalarSoni { get; set; }
        public string? Nashriyot { get; set; }
        public string? Shahar { get; set; }

        [StringLength(50)]
        public string? Til { get; set; } = "uz";

        [StringLength(50)]
        public string? UDK { get; set; }

        [StringLength(50)]
        public string? BBK { get; set; }

        [StringLength(50)]
        public string? MuallifBelgisi { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Narxi { get; set; }

        public IFormFile? RasmFayl { get; set; }
        public string? Rasm { get; set; } // bazada saqlanadigan path

        public int NusxalarSoni { get; set; } = 1;
        public MediaType MediaTuri { get; set; } = MediaType.Kitob;

        public bool Elektron { get; set; } = false;
        public IFormFile? ElektronFayl { get; set; }
        public string? ElektronManzil { get; set; }
        public string? FaylFormati { get; set; }

        // Many-to-many uchun tanlangan ID'lar
        public List<int> SelectedAuthorIds { get; set; } = new List<int>();
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        // Tanlash uchun ro‘yxatlar
        public List<SelectListItem> AvailableAuthors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();
    }
}