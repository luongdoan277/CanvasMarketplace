using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using CanvasMarketplace.Areas.Admin.DTO;

namespace CanvasMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RoleAccess")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to display a list of categories
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _context.Categories
                    .Select(c => new CategoryDTO()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Created = c.CreatedDate
                    })
                    .ToListAsync();

                return View(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display details of a category
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || !_context.Categories.Any())
                {
                    return NotFound();
                }

                var category = await _context.Categories
                    .Where(c => c.Id == id)
                    .Select(c => new CategoryDTO()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Created = c.CreatedDate
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display the category creation form
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle category creation
        // Action to handle category creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cat = new Category()
                    {
                        CreatedDate = DateTime.Now,
                        Name = category.Name
                    };
                    _context.Add(cat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(category); // Return the same view with validation errors
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Action to display the category editing form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                var categoryDTO = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Created = category.CreatedDate
                };

                return View(categoryDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle category editing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryDTO category)
        {
            try
            {
                if (id != category.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var cat = new Category()
                    {
                        Id = category.Id,
                        CreatedDate = category.Created,
                        Name = category.Name
                    };

                    _context.Update(cat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Action to display the category deletion confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || !_context.Categories.Any())
                {
                    return NotFound();
                }

                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                var cat = new CategoryDTO()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Created = category.CreatedDate
                };

                return View(cat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle category deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Categories == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Categories' is null.");
                }

                var category = await _context.Categories.FindAsync(id);

                if (category != null)
                {
                    _context.Categories.Remove(category);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories?.Any(e => e.Id == id) ?? false;
        }
    }
}
