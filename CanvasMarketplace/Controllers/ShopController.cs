using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using CanvasMarketplace.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CanvasMarketplace.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShopController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;

        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? id)
        {
            try
            {
                IQueryable<ProductDTO> productsQuery = _context.Products
                .Select(p => new ProductDTO()
                {
                    Id = p.Id,
                    Description = p.Description,
                    Name = p.Name,
                    Price = p.Price,
                    ImageThumbnailUrl = "/images/" + p.ImageThumbnailUrl,
                    ImageUrl = "/images/" + p.ImageUrl,
                    Status = p.Status,
                    CatId = p.CategoryId,
                    UerId = p.UserId
                })
                .Where(el => el.Status == true);

                if (id.HasValue && id.Value != 0)
                {
                    productsQuery = productsQuery.Where(p => p.CatId == id.Value);
                }

                List<ProductDTO> products = await productsQuery
                    .Take(10) // Giới hạn số lượng sản phẩm từ 1 đến 10
                    .ToListAsync();
                ViewData["ShowCarousel"] = true;
                return View(products);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDTO()
                {
                    Id = product.Id,
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    ImageThumbnailUrl = "/images/" + product.ImageThumbnailUrl,
                    ImageUrl = "/images/" + product.ImageUrl,
                    Status = product.Status,
                    CatId = product.CategoryId,
                    UerId = product.UserId
                };


                var checkUser = _userManager.GetUserId(User);

                if (checkUser == null)
                {
                    ViewBag.Auth = false;
                }
                else
                {
                    ViewBag.Auth = true;

                }

                return View(productDto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
    }
}
