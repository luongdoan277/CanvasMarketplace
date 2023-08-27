using CanvasMarketplace.Data.Entities;

namespace CanvasMarketplace.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CatId { get; set; }

        public Guid UerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; } = decimal.Zero;

        public string ImageUrl { get; set; } = string.Empty;

        public string ImageThumbnailUrl { get; set; } = string.Empty;

        public bool Status { get; set; }

    }
}
