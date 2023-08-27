using CanvasMarketplace.Areas.Admin.DTO;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CanvasMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RoleAccess")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            // Retrieve product information with DTO transformation
            var products = await _context.Products
                .Select(p => new ProductDTO()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageThumbnailUrl = "/images/" + p.ImageThumbnailUrl,
                    ImageUrl = "/images/" + p.ImageUrl,
                    Price = p.Price,
                    Status = p.Status
                })
                .ToListAsync();

            return View(products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Select(p => new ProductDTO()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageThumbnailUrl = "/images/" + p.ImageThumbnailUrl,
                    ImageUrl = "/images/" + p.ImageUrl,
                    Price = p.Price,
                    Status = p.Status
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            // Provide select list for category ID only
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var newProduct = new Product()
                {
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    ImageUrl = UploadedFile(product.ImageData),
                    ImageThumbnailUrl = UploadedFile(product.ImageThumbnailData),
                    CategoryId = product.CategoryId,
                    UserId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier))
                };

                _context.Add(newProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Provide select list for category ID on validation error
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Products.FindAsync(id);
            if (p == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", p.CategoryId);
            var productDTO = new UpdatePostProductDTO()
            {
                Id = p.Id,
                Status = p.Status, // Set the Status field
            };

            return View(productDTO);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePostProductDTO product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    existingProduct.Status = product.Status; // Update the Status field

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }


        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var productDTO = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageThumbnailUrl = "/images/" + product.ImageThumbnailUrl,
                ImageUrl = "/images/" + product.ImageUrl,
                Price = product.Price,
                Status = product.Status
            };

            return View(productDTO);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        // Other actions (Edit, Delete, etc.) remain mostly unchanged...

        private string UploadedFile(IFormFile model)
        {
            string uniqueFileName = null;

            if (model != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
