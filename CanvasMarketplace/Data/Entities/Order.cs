using CanvasMarketplace.Data.Enums;

namespace CanvasMarketplace.Data.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; } // Foreign key matching a user

        public decimal TotalAmount { get; set; } // Total amount of the order

        public string Street { get; set; }

        public StatusOrder Status { get; set; } // Status of the order

        public AppUser AppUser { get; set; } = new AppUser(); // User associated with the order

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // List of order items in the order
    }
}
