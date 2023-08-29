using CanvasMarketplace.Data.Enums;
using CanvasMarketplace.DTO;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Street { get; set; }
        public StatusOrder Status { get; set; }
        public AppUserDTO AppUser { get; set; }
        public List<OrderItemInOrderDTO> OrderItems { get; set; }
    }

    public class OrderItemInOrderDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ProductImageThumbnailUrl { get; set; }
        public string ProductName { get; set; }
        public ProductCategoryDTO ProductCategory { get; set; }
    }

    public class AppUserDTO
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ProductCategoryDTO
    {
        public string Name { get; set; }
    }
}
