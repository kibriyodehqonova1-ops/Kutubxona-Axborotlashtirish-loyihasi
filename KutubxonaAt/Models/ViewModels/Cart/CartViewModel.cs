namespace KutubxonaAt.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<Bron> Holds { get; set; } = [];          // Faol bronlar
        public List<Beruv> Checkouts { get; set; } = [];     // Berilgan kitoblar
        public List<Sotuv> eBooks { get; set; } = [];        // Sotib olingan elektron kitoblar
        public List<Jarima> Fines { get; set; } = [];        // Kutilayotgan jarimalar
    }
}
