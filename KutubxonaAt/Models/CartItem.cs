namespace KutubxonaAt.Models
{
    public class CartItem
    {
        public int KitobId { get; set; }
        public string Nomi { get; set; } = string.Empty;
        public string? Rasm { get; set; }
        public decimal Narxi { get; set; }
        public int KitobNusxaId { get; set; }
        public string Mualliflar { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
    }
}