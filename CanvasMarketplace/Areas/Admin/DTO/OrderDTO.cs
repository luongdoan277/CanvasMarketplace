using CanvasMarketplace.Data.Enums;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Street { get; set; }
        public StatusOrder Status { get;set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
    }
}
