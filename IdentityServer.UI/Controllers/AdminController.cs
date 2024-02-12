using IdentityServer.Core.Dto;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Data.Models;
using IdentityServer.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Microsoft.AspNetCore.Components.Route("admin")]
[Authorize(Roles = "IDENTITY_ADMIN")]
public class AdminController : Controller {

    private readonly IUserRepository _userRepository;
    public AdminController(IUserRepository userRepository) {
        _userRepository = userRepository;
    }

    public IActionResult Index() {
        return View();
    }

    [Route("users")]
    public async Task<IActionResult> UsersDashboard() {
        var users = await _userRepository.GetUsersAsync();
        return View(users);
    }

    [Route("api/users/{id}/roles")]
    public async Task<IActionResult> UserRoles(string id) {
        var allRoles = await _userRepository.GetAllRolesAsync();
        var roles = await _userRepository.GetUserRolesAsync(id);
        return PartialView("_RolesDisplay", new RoleDialogViewModel {AllRoles = allRoles, UserRoles = roles, UserId = id});
    }

    [Route("api/users/{userId}/roles/{roleName}")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName) {
        var res = await _userRepository.RemoveUserFromRoleAsync(userId, roleName);
        if (res.IsFaulted) {
            return BadRequest();
        }

        return await UserRoles(userId);
    }
    
    [Route("api/users/{userId}/roles/{roleName}")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(string userId, string roleName) {
        var res = await _userRepository.AddUserToRoleAsync(userId, roleName);
        if (res.IsFaulted) {
            return BadRequest();
        }

        return await UserRoles(userId);
    }
    
    public IActionResult RolesDashboard() {
        return NotFound();
    }
}