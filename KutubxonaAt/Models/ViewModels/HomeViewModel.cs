using System.Collections.Generic;

namespace KutubxonaAt.Models.ViewModels
{
    public class HomeViewModel
    {
        public string LeadText { get; set; } = string.Empty;
        public string WelcomeText { get; set; } = string.Empty;
        public int EBookCount { get; set; }
        public int AudioBookCount { get; set; }
        public int JournalCount { get; set; }
        public int VideoCount { get; set; }

        public List<BookCardViewModel> RecentBooks { get; set; } = new();
        public bool IsSearch { get; set; }
        public List<BookCardViewModel> SearchResults { get; set; } = new();
        public string SearchKeyword { get; set; } = string.Empty;
        public int? SelectedCatalog { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}