using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class CreateProductDTO
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

        [Required(ErrorMessage = "Image is required.")]
        public IFormFile ImageData { get; set; }

        [Required(ErrorMessage = "Thumbnail Image is required.")]
        public IFormFile ImageThumbnailData { get; set; }

        public bool Status { get; set; }
    }
}
