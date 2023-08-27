using System;
using System.ComponentModel.DataAnnotations;

namespace CanvasMarketplace.Areas.Admin.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.Now;
    }
}
