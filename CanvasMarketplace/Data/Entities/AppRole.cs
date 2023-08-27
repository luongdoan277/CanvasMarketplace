using Microsoft.AspNetCore.Identity;

namespace CanvasMarketplace.Data.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = string.Empty;
    }
}
