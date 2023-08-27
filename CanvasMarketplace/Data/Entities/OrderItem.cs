using CanvasMarketplace.Data.Enums;

namespace CanvasMarketplace.Data.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = new Product();
        public Order Order { get; set; } = new Order();
    }
}
