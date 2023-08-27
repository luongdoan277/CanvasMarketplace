namespace CanvasMarketplace.Data.Entities
{
    public class Product : BaseEntity
    {
        public int CategoryId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = decimal.Zero;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageThumbnailUrl { get; set; } = string.Empty;
        public bool Status { get; set; }
        public Category Category { get; set; } = new Category();
        public AppUser AppUser { get; set; } = new AppUser();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
