using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string TicketCode { get; set; }
        public bool IsCheckedIn { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }
        public int TicketId { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}