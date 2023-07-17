using JV_Imprest_Payment.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace JV_Imprest_Payment.Controllers
{
    [Authorize(Roles ="Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get all users and roles
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            // Create a view model to pass to the view
            var viewModel = new AdminViewModel
            {
                Users = users,
                Roles = roles,
                UserRoles = new Dictionary<string, List<string>>()
            };

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                viewModel.UserRoles[user.Id] = userRoles.ToList();
            }

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AssignRole(Dictionary<string, List<string>> selectedRoles)
        {
            foreach (var userId in selectedRoles.Keys)
            {
                // Find the user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // Handle the case when the user doesn't exist
                    // For example, you can display an error message or redirect back to the index page with a notification
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get the current roles of the user
                var userRoles = await _userManager.GetRolesAsync(user);

                // Get the selected roles for the user
                var rolesToAdd = selectedRoles[userId];
                var rolesToRemove = userRoles.Except(rolesToAdd);

                // Add roles to the user
                await _userManager.AddToRolesAsync(user, rolesToAdd);

                // Remove roles from the user
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            return RedirectToAction(nameof(Index));
        }


    }

}
