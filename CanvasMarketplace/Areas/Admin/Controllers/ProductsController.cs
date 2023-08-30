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
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            try
            {
                // Provide select list for category ID only
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDTO product)
        {
            try
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
                    var category = await _context.Categories.FindAsync(product.CategoryId);

                    if (category != null)
                    {
                        newProduct.Category = category;
                    }

                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                // Provide select list for category ID on validation error
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Admin/Products/Post/5
        public async Task<IActionResult> Post(int? id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: Admin/Products/Post/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(int id, UpdatePostProductDTO product)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<IActionResult> Edit(int? id)
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

                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                var productDTO = new UpdateProductDTO()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    ImageThumbnailUrl = "/images/" + product.ImageThumbnailUrl,
                    ImageUrl = "/images/" + product.ImageUrl,
                    CategoryId = product.CategoryId,
                    UserId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    IsUpdateImage= false,
                    IsUpdateImageThumbnail = false

                };

                return View(productDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDTO product)
        {
            try
            {
                if (id != product.Id)
                {
                    return NotFound();
                }
                // Xóa lỗi validation cho các trường không cần kiểm tra
                ModelState.Remove("ImageData");
                ModelState.Remove("ImageThumbnailData");
                if (ModelState.IsValid)
                {
                    try
                    {
                        string urlUpdateImage = product.ImageUrl?.Replace("/images/", "");
                        string urlUpdateImageThumbnail = product.ImageThumbnailUrl?.Replace("/images/", "");


                        if (product.IsUpdateImage)
                        {
                            DeleteImageFile(urlUpdateImage);
                            urlUpdateImage = UploadedFile(product.ImageData);
                        }
                        if (product.IsUpdateImageThumbnail)
                        {
                            DeleteImageFile(urlUpdateImageThumbnail);
                            urlUpdateImageThumbnail = UploadedFile(product.ImageThumbnailData);
                        }

                        var category = await _context.Categories.FindAsync(product.CategoryId);
                        var existingProduct = await _context.Products.FindAsync(id);

                        if (existingProduct == null)
                        {
                            return NotFound();
                        }
                        if (category != null)
                        {
                            existingProduct.Category = category;
                        }

                        existingProduct.CategoryId = product.CategoryId;
                        existingProduct.ImageUrl = urlUpdateImage;
                        existingProduct.ImageThumbnailUrl = urlUpdateImageThumbnail;
                        existingProduct.Name = product.Name;
                        existingProduct.Price = product.Price;
                        existingProduct.Description = product.Description;

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
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                DeleteImageFile(product.ImageThumbnailUrl);
                DeleteImageFile(product.ImageUrl);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        // Other actions (Edit, Delete, etc.) remain mostly unchanged...

        private string UploadedFile(IFormFile model)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void DeleteImageFile(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
