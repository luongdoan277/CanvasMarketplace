using CanvasMarketplace.Areas.Admin.DTO;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using CanvasMarketplace.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CanvasMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RoleAccess")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to display a list of orders
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _context.Orders.Include(o => o.AppUser).Select(o => new OrderDTO()
                {
                    Id = o.Id,
                    Email = o.AppUser.Email,
                    Created = o.CreatedDate,
                    Status = o.Status,
                    Street = o.Street,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display order details
        public IActionResult Details(int Id)
        {
            try
            {
                var order = _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefault(o => o.Id == Id);

                if (order == null)
                {
                    return NotFound();
                }

                var orderDetailDTO = new OrderDetailDTO
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    AppUser = new AppUserDTO
                    {
                        FirstName = order.AppUser.FirstName,
                        Email = order.AppUser.Email,
                        PhoneNumber = order.AppUser.PhoneNumber
                    },
                    OrderItems = order.OrderItems.Select(oi => new OrderItemInOrderDTO
                    {
                        Id = oi.Id,
                        Price = oi.Product.Price,
                        ProductImageThumbnailUrl = "/images/" + oi.Product.ImageThumbnailUrl,
                        ProductName = oi.Product.Name,
                        ProductCategory = new ProductCategoryDTO
                        {
                            Name = oi.Product.Category.Name
                        }
                    }).ToList()
                };

                return View(orderDetailDTO);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // Action to display the order creation form
        public IActionResult Create()
        {
            try
            {
                ViewData["UerId"] = new SelectList(_context.AppUsers, "Id", "Id");
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle order creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UerId,TotalAmount,Status,Id,CreatedDate")] Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Id", order.UserId);
                return View(order);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display the order editing form
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var order = _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefault(o => o.Id == id);

                if (order == null)
                {
                    return NotFound();
                }

                var orderDetailDTO = new OrderDetailDTO
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    Street = order.Street,
                    AppUser = new AppUserDTO
                    {
                        FirstName = order.AppUser.FirstName,
                        Email = order.AppUser.Email,
                        PhoneNumber = order.AppUser.PhoneNumber
                    },
                    OrderItems = order.OrderItems.Select(oi => new OrderItemInOrderDTO
                    {
                        Id = oi.Id,
                        Price = oi.Product.Price,
                        ProductImageThumbnailUrl = "/images/" + oi.Product.ImageThumbnailUrl,
                        ProductName = oi.Product.Name,
                        ProductCategory = new ProductCategoryDTO
                        {
                            Name = oi.Product.Category.Name
                        }
                    }).ToList()
                };

                return View(orderDetailDTO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle order editing
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int orderId, StatusOrder newStatus)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                {
                    return NotFound();
                }

                order.Status = newStatus;

                _context.Update(order);

                _context.SaveChanges();

                // Redirect to the order detail page or order list page
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display the order deletion confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var order = await _context.Orders.Include(o => o.AppUser)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle order deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order != null)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Helper method to check if an order exists
        private bool OrderExists(int id)
        {
            return _context.Orders?.Any(e => e.Id == id) ?? false;
        }
    }
}
