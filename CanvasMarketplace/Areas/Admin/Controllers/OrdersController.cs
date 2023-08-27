using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
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
            var orders = await _context.Orders.Include(o => o.AppUser).ToListAsync();
            return View(orders);
        }

        // Action to display order details
        public async Task<IActionResult> Details(int? id)
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

        // Action to display the order creation form
        public IActionResult Create()
        {
            ViewData["UerId"] = new SelectList(_context.AppUsers, "Id", "Id");
            return View();
        }

        // Action to handle order creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UerId,TotalAmount,Status,Id,CreatedDate")] Order order)
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

        // Action to display the order editing form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["UerId"] = new SelectList(_context.AppUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // Action to handle order editing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UerId,TotalAmount,Status,Id,CreatedDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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

            ViewData["UerId"] = new SelectList(_context.AppUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // Action to display the order deletion confirmation page
        public async Task<IActionResult> Delete(int? id)
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

        // Action to handle order deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        // Helper method to check if an order exists
        private bool OrderExists(int id)
        {
            return _context.Orders?.Any(e => e.Id == id) ?? false;
        }
    }
}
