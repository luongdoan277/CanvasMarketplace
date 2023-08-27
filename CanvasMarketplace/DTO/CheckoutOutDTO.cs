namespace CanvasMarketplace.DTO
{
    public class CheckoutOutDTO
    {
        public UserDTO User { get; set; }
        public List<OrderItemDTO> OrderItems { get; set;}
    }
}
