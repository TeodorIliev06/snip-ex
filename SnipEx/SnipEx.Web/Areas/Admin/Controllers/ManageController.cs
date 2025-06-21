namespace SnipEx.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SnipEx.Services.Data.Contracts;

    using static Common.ApplicationConstants;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class ManageController(
        IManagerService adminService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await adminService.GetAllUsersWithBanStatusAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBan(string userId)
        {
            var result = await adminService.ToggleBanStatusAsync(userId);

            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
