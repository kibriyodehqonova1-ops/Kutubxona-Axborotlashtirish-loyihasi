namespace KutubxonaAt.Services
{
    public interface IBildirishnomaService
    {
        Task YuborAsync(string foydalanuvchiId, string sarlavha, string matn,
            string tur = "Umumiy", string? havola = null);

        Task KopYuborAsync(IEnumerable<string> foydalanuvchiIdlar, string sarlavha,
            string matn, string tur = "Umumiy", string? havola = null);

        Task AbonementEslatmaYuborAsync();
        Task QaytarishEslatmaYuborAsync();
    }
}
