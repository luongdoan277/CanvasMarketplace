namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;


        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; } = decimal.Zero;

        public string ImageUrl { get; set; }


        public string ImageThumbnailUrl { get; set; }

        public IFormFile ImageData { get; set; }


        public IFormFile ImageThumbnailData { get; set; }
        

        public bool Status { get; set; }
    }
}
