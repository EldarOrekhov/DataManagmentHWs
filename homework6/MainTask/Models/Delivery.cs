using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string Requisites { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime ReceivingDate { get; set; }
    }
}
