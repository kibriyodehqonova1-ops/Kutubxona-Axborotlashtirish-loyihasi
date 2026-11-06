namespace KutubxonaAt.Models
{
    public class KitobKategoriya
    {
        public int KitobId { get; set; }
        public int KategoriyaId { get; set; }

        public virtual Kitob Kitob { get; set; } = null!;
        public virtual Kategoriya Kategoriya { get; set; } = null!;
    }
}