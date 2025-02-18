namespace FinanceManagmentApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public UserSettings Settings { get; set; } = null!;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
