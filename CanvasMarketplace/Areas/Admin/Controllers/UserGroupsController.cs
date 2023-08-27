using CanvasMarketplace.Areas.Admin.DTO;
using CanvasMarketplace.Configurations;
using CanvasMarketplace.Data.EF;
using CanvasMarketplace.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CanvasMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Manager")]
    public class UserGroupsController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly ILogger<UserGroupsController> logger;
        private readonly ApplicationDbContext context;

        public UserGroupsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ILogger<UserGroupsController> _logger,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = _logger;
            this.context = context;
        }

        //// <summary>
        /// Display a list of user groups (excluding Admins).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userGroups = await context.Users
                    .Join(context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                    .Select(c => new UserGroupsDTO()
                    {
                        Id = c.ur.u.Id,
                        UserName = c.ur.u.UserName,
                        Email = c.ur.u.Email,
                        Role = c.r.Name
                    })
                    .Where(el => el.Role != UserRoles.Admin)
                    .ToListAsync();

                return View(userGroups);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Display details of a user group.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if (id == null || context.AppUsers == null)
                {
                    return NotFound();
                }

                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }
                var roles = await userManager.GetRolesAsync(user);

                var result = new UserDTO()
                {
                    Role = roles.FirstOrDefault(),
                    Name = user.FirstName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                return View(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Display the user group creation form.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Roles = new List<string>() { UserRoles.User, UserRoles.Client };
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Handle user group creation.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDTO request)
        {
            ViewBag.Roles = new List<string>() { UserRoles.User, UserRoles.Client };
            if (ModelState.IsValid)
            {
                // Check if a user with the given email already exists
                var userCheck = await userManager.FindByEmailAsync(request.Email);

                if (userCheck == null)
                {
                    // Create a new user instance based on the provided request data
                    var user = new AppUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        FirstName = request.Name,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };

                    // Attempt to create the user in the identity system
                    var result = await userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        // Create and assign the user role if it doesn't exist
                        if (request.Role == UserRoles.User)
                        {
                            await CreateAndAssignRoleIfNotExists(UserRoles.User, user);
                        }
                        // Create and assign the client role if it doesn't exist
                        else if (request.Role == UserRoles.Client)
                        {
                            await CreateAndAssignRoleIfNotExists(UserRoles.Client, user);
                        }

                        // Redirect to the Index action
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Add identity error messages to the ModelState for display in the view
                        AddErrorsToModelState(result.Errors);
                        return View(request);
                    }
                }
                else
                {
                    // If a user with the provided email already exists, add a custom error message
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }

            // If the ModelState is invalid, return the view with the provided request data
            return View(request);
        }


        // Action to display the user group deletion confirmation
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || context.AppUsers == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var roles = await userManager.GetRolesAsync(user);

            var result = new UserDTO()
            {
                Id = id,
                Role = roles.FirstOrDefault(),
                Name = user.FirstName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(result);
        }

        // Action to handle user group deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (context.AppUsers == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.AppUsers' is null.");
                }

                var appUser = await context.AppUsers.FindAsync(id);
                if (appUser != null)
                {
                    context.AppUsers.Remove(appUser);
                }

                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Helper method to create and assign a role if it doesn't exist
        private async Task CreateAndAssignRoleIfNotExists(string roleName, AppUser user)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new AppRole { Name = roleName };
                await roleManager.CreateAsync(role);
            }
            await userManager.AddToRoleAsync(user, roleName);
        }

        // Helper method to add errors to ModelState
        private void AddErrorsToModelState(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("message", error.Description);
            }
        }

        // Helper method to check if an AppUser with a given ID exists
        private bool AppUserExists(Guid id)
        {
            return (context.AppUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
