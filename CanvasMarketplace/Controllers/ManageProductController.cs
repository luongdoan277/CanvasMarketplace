using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using CanvasMarketplace.Data.Enums;
using CanvasMarketplace.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CanvasMarketplace.Controllers
{
    [Authorize(Policy = "RoleAccess")]
    public class ManageProductController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext context;

        public ManageProductController(
            UserManager<AppUser> userManager,
            ApplicationDbContext context)
        {

            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> Cart()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<IActionResult> CheckoutOut()
        {
            try
            {
                var user = await this.userManager.GetUserAsync(User);

                var userDTO = new UserDTO()
                {
                    FullName = user.FirstName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };

                var result = new CheckoutOutDTO() { User = userDTO };

                return View(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckoutOut(CheckoutOutDTO model)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                decimal totalAmount = model.OrderItems.Sum(item => item.Total);

                var newOrder = new Order()
                {
                    UserId = user.Id,
                    TotalAmount = totalAmount,
                    Status = StatusOrder.Pending, // Set the initial status here
                    Street = model.User.Street
                };

                var newUser = await context.AppUsers.FindAsync(user.Id);

                if (newUser != null)
                {
                    newOrder.AppUser = newUser;
                }

                var newOrderItems = new List<OrderItem>();

                foreach (var item in model.OrderItems)
                {
                    var newOrderItem = new OrderItem()
                    {
                        ProductId = item.ProId,
                        Quantity = item.Quantity
                    };

                    var product = await context.Products.FindAsync(item.ProId);

                    if (product != null)
                    {
                        newOrderItem.Product = product;
                        newOrderItems.Add(newOrderItem);
                    }
                }

                newOrder.OrderItems = newOrderItems;

                context.Orders.Add(newOrder);

                await context.SaveChangesAsync();

                // Trả về kết quả thành công
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có

                // Trả về kết quả thất bại và thông tin lỗi
                return Ok(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation()
        {
           
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
