namespace KutubxonaAt.Models
{
    public class KitobMuallif
    {
        public int KitobId { get; set; }
        public int MuallifId { get; set; }

        public virtual Kitob Kitob { get; set; } = null!;
        public virtual Muallif Muallif { get; set; } = null!;
    }
}