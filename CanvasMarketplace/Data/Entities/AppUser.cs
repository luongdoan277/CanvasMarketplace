using Microsoft.AspNetCore.Identity;
using System;

namespace CanvasMarketplace.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Dob { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Product> Product { get; set; } = new List<Product>();
    }
}
