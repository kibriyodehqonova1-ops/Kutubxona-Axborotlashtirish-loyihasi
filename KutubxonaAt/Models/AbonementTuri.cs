using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    /// <summary>
    /// Abonement (obuna) turlari: oylik, choraklik, yillik
    /// </summary>
    public class AbonementTuri
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Abonement nomi majburiy")]
        [StringLength(100)]
        public string Nomi { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Tavsif { get; set; }

        [Required]
        public decimal Narxi { get; set; }

        /// <summary>Abonement davomiyligi (kunlarda)</summary>
        [Required]
        public int DavomiylikKun { get; set; } = 30;

        /// <summary>Oyiga bepul olish mumkin bo'lgan kitoblar soni (0 = cheksiz)</summary>
        public int BepulKitoblarSoni { get; set; } = 5;

        /// <summary>Chegirma foizi (0-100)</summary>
        public decimal ChegirimFoizi { get; set; } = 0;

        /// <summary>Yetkazib berish bepulmi</summary>
        public bool BepulYetkazish { get; set; } = false;

        /// <summary>Bir vaqtda nechta kitob olish mumkin</summary>
        public int MaksimalKitoblar { get; set; } = 5;

        public bool Faol { get; set; } = true;

        public DateTime YaratilganSana { get; set; } = DateTime.Now;

        // Navigation
        public virtual ICollection<Abonement> Abonementlar { get; set; } = new List<Abonement>();
    }
}
