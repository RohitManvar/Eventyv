using System;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public int QuantitySold { get; set; }


        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        // Foreign Key
        public int EventId { get; set; }

        // Navigation Property
        public virtual Event Event { get; set; }

        // Optional helper
        public bool IsSaleDateValid => !SaleEndDate.HasValue || SaleStartDate <= SaleEndDate;

        public bool IsFree { get; set; }  // now you can assign to it

    }

    public enum TicketType
    {
        Free,
        Paid,
        Donation
    }
}
