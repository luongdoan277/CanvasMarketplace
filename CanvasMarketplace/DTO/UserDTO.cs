using System.ComponentModel.DataAnnotations;

namespace CanvasMarketplace.DTO
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Street Address is required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
    }

}
