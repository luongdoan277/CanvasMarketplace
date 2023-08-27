using CanvasMarketplace.Configurations;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using CanvasMarketplace.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanvasMarketplace.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ILogger<AccountController> logger;
        private readonly ApplicationDbContext context;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ILogger<AccountController> _logger,
            ApplicationDbContext context)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = _logger;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginDTO model = new DTO.LoginDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");

                    return View(model);
                }

                bool checkPass = await userManager.CheckPasswordAsync(user, model.Password);


                if (!checkPass)
                {
                    ModelState.AddModelError("message", "Invalid credentials");

                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(
                        model.Email,
                        model.Password,
                        model.RememberMe,
                        true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Shop");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            CreateUserDTO model = new CreateUserDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserDTO request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);

                if (userCheck == null)
                {
                    var user = new AppUser
                    {
                        FirstName = request.Name,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };

                    var result = await userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        var role = new AppRole();

                        if (!await roleManager.RoleExistsAsync(UserRoles.Client))
                        {
                            role.Name = UserRoles.Client;
                            await roleManager.CreateAsync(role);
                        }
                        await userManager.AddToRoleAsync(user, UserRoles.Client);

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // Đăng xuất người dùng
            return RedirectToAction("Index", "Shop"); // Chuyển hướng về trang chủ hoặc trang khác
        }

        public async Task<IActionResult> History()
        {

            var result = await (from od in context.Orders
                        join oi in context.OrderItems on od.Id equals oi.OrderId
                        join p in context.Products on oi.ProductId equals p.Id
                        where od.UserId.ToString() == userManager.GetUserId(User)
                         select new { od, oi, p })
                        .Select(el => new HistoryDTO()
                        {
                            Product = el.p.Name,
                            Price = el.p.Price,
                            Quantity = el.oi.Quantity,
                            IsActive = el.od.Status,
                            TotalPrice = el.od.TotalAmount
                        })
                        .ToListAsync();

            return View(result);
        }

    }
}
