namespace KutubxonaAt.ViewModels.User
{
    public class UserIndexViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Ism { get; set; } = string.Empty;
        public string Familiya { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefon { get; set; }
        public string Turi { get; set; } = string.Empty;
        public bool Faol { get; set; }
        public DateTime? AmalMuddati { get; set; }
        public string Roles { get; set; } = string.Empty;
    }
}