namespace KutubxonaAt.Models.ViewModels
{
    public class BookCardViewModel
    {
        public int Id { get; set; }
        public string Nomi { get; set; } = string.Empty;
        public string Rasm { get; set; } = string.Empty;
        public string Mualliflar { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public double OrtachaBaho { get; set; }
        public string Kategoriyalar { get; set; } = string.Empty;
        public string MediaTuri { get; set; } = string.Empty;
        public string Tavsif { get; set; } = string.Empty;
        public string FilterClasses { get; set; } = string.Empty;
    }
}