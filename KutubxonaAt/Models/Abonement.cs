using System.ComponentModel.DataAnnotations;

namespace KutubxonaAt.Models
{
    /// <summary>
    /// Foydalanuvchi abonement (obuna) yozuvi
    /// </summary>
    public class Abonement
    {
        public int Id { get; set; }

        [Required]
        public string FoydalanuvchiId { get; set; } = string.Empty;

        [Required]
        public int AbonementTuriId { get; set; }

        public DateTime BoshlanishSana { get; set; } = DateTime.Now;
        public DateTime TugashSana { get; set; }

        public decimal TolanganNarx { get; set; }

        public string Holati { get; set; } = "Faol"; // Faol, Tugagan, Bekor qilingan

        public string? TolovUsuli { get; set; } // Naqd, Click, Payme, Bank

        public string? TolovId { get; set; } // Tashqi to'lov tizimi ID

        /// <summary>Joriy oyda ishlatilgan bepul kitoblar soni</summary>
        public int IshlatilganBepulKitoblar { get; set; } = 0;

        /// <summary>Eslatma: tugashiga 3 kun qolganda yuborilganmi</summary>
        public bool Eslatma3KunYuborildi { get; set; } = false;

        /// <summary>Eslatma: tugaganidan keyin yuborilganmi</summary>
        public bool EslatmaTugaganYuborildi { get; set; } = false;

        public DateTime YaratilganSana { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Izoh { get; set; }

        // Navigation
        public virtual ApplicationUser? Foydalanuvchi { get; set; }
        public virtual AbonementTuri? AbonementTuri { get; set; }

        // Helper
        public bool Faolmi => Holati == "Faol" && TugashSana > DateTime.Now;
        public int QolganKunlar => Faolmi ? (int)(TugashSana - DateTime.Now).TotalDays : 0;
    }
}
