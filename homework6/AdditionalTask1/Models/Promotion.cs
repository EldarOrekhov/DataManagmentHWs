namespace ConsoleApp1.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Percent { get; set; }
        public decimal? Amount { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }
        public override string ToString()
        {
            return String.Format("Name - {0}\nDiscount - {1}",Name, Percent ?? Amount);
        }
    }
}