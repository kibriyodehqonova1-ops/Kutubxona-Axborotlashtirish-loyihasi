using System.ComponentModel.DataAnnotations.Schema;

namespace KutubxonaAt.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int KitobId { get; set; }
        public int KitobNusxaId { get; set; }
        public decimal Narxi { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; }

        [ForeignKey(nameof(KitobId))]
        public virtual Kitob? Kitob { get; set; }

        [ForeignKey(nameof(KitobNusxaId))]
        public virtual KitobNusxa? KitobNusxa { get; set; }
    }
}