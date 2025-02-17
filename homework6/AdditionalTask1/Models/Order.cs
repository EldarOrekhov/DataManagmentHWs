namespace ConsoleApp1.Models
{
    public class Order 
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } 
        public string City { get; set; } 
        public string Address { get; set; } 
        public bool Shipped { get; set; } 
        public virtual ICollection<OrderLine> Lines { get; set; } 
    }
    public class OrderLine 
    { 
        public int Id { get; set; } 
        public int BookId { get; set; }
        public int OrderId { get; set; } 
        public int Quantity { get; set; }
        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}