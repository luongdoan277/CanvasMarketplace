using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using CanvasMarketplace.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CanvasMarketplace.Controllers
{
    [Authorize(Policy = "RoleAccess")]
    public class ManageProductController: Controller
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

        public IActionResult Cart()
        {
            return View();
        }

       
        public async Task<IActionResult> CheckoutOut()
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
        [HttpPost]
        public async Task<IActionResult> CheckoutOut(CheckoutOutDTO model)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                decimal totalAmount = 0;
                foreach (var item in model.OrderItems)
                {
                    totalAmount += item.Total ;
                }

                var order = new Order()
                {
                    UserId = user.Id,
                    TotalAmount = totalAmount
                };

                await context.AddAsync(order);

                foreach (var item in model.OrderItems)
                {
                    var orderItem = new OrderItem()
                    {
                        OrderId = order.Id,
                        ProductId = item.ProId,
                        Quantity = item.Quantity
                    };

                    await context.AddAsync(orderItem);
                }

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
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}
