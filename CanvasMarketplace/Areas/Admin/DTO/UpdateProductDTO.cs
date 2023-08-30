using System.ComponentModel.DataAnnotations;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class UpdateProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; } = decimal.Zero;
        
        public IFormFile ImageData { get; set; }
        
        public IFormFile ImageThumbnailData { get; set; }

        public string ImageUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }
        public bool IsUpdateImage { get; set; }
        public bool IsUpdateImageThumbnail { get; set; }

        public bool Status { get; set; }
    }
}
