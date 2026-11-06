namespace KutubxonaAt.ViewModels
{
    public class RecentBookViewModel
    {
        public int Id { get; set; }
        public string Nomi { get; set; } = string.Empty;
        public string? Rasm { get; set; }
        public string? ISBN { get; set; }
        public decimal? Narxi { get; set; }
        public double OrtachaBaho { get; set; }
        public string Mualliflar { get; set; } = string.Empty;
        public string Kategoriyalar { get; set; } = string.Empty;
        public string MediaTuri { get; set; } = string.Empty;
        // Filtrlash uchun CSS klasslari (vergul bilan ajratilgan)
        public string FilterClasses { get; set; } = string.Empty;
    }
}