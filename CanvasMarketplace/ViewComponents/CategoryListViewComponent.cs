using CanvasMarketplace.Data.EF;
using CanvasMarketplace.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanvasMarketplace.ViewComponents
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryListViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CategoryDTO> categories = await _context.Categories.Select(x => new CategoryDTO()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            return View(categories);
        }
    }
}
