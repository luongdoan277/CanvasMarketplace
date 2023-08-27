using CanvasMarketplace.Configurations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class UserDTO
    {
        public Guid? Id { get; set; }

        [DisplayName("Full Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        public string Password { get; set; }
       
        public string ConfirmPassword { get; set; }

        [DisplayName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DisplayName("Role")]
        public string Role { get; set; } = UserRoles.Admin;
    }
}
