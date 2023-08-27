using CanvasMarketplace.Data.Enums;

namespace CanvasMarketplace.DTO
{
    public class HistoryDTO
    {
        public string Product { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public StatusOrder IsActive { get; set; }
    }
}
