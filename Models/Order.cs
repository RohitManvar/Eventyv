using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public string BuyerEmail { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}