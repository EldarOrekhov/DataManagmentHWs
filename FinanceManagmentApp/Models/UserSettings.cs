namespace FinanceManagmentApp.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string Currency { get; set; } = "USD";

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
