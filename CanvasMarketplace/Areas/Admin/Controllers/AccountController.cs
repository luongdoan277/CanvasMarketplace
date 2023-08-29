using CanvasMarketplace.Areas.Admin.DTO;
using CanvasMarketplace.Configurations;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CanvasMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
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

        // Action to display the registration form
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            try
            {
                CreateUserDTO model = new CreateUserDTO();
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle user registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserDTO request)
        {
            try
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
                            // Create and assign roles based on user's choice
                            var role = new AppRole();
                            if (request.Role == UserRoles.User)
                            {
                                await CreateAndAssignRoleIfNotExists(UserRoles.User, user);
                            }

                            else if (request.Role == UserRoles.Admin)
                            {
                                await CreateAndAssignRoleIfNotExists(UserRoles.Admin, user);
                            }

                            return RedirectToAction("Login");
                        }
                        else
                        {
                            AddErrorsToModelState(result.Errors);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to display the login form
        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                LoginDTO model = new LoginDTO();
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle user login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            try
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
                        var roles = await userManager.GetRolesAsync(user);

                        // Redirect users based on their roles
                        if (roles.FirstOrDefault() == UserRoles.Admin)
                        {
                            return RedirectToAction("Index", "UserGroups");
                        }
                        else if (roles.FirstOrDefault() == UserRoles.User)
                        {
                            return RedirectToAction("Index", "Orders");
                        }
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Action to handle user logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync(); // Đăng xuất người dùng
                return RedirectToAction("Login"); // Chuyển hướng về trang chủ hoặc trang khác
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Helper method to create and assign a role if it doesn't exist
        private async Task CreateAndAssignRoleIfNotExists(string roleName, AppUser user)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new AppRole { Name = roleName };
                    await roleManager.CreateAsync(role);
                }
                await userManager.AddToRoleAsync(user, roleName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Helper method to add errors to ModelState
        private void AddErrorsToModelState(IEnumerable<IdentityError> errors)
        {
            try
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("message", error.Description);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
