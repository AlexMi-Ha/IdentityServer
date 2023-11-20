using System.Security.Claims;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

// TODO: Authorize
[Route("user")]
public class UserController(IUserRepository userRepo) : Controller {
    
    public IActionResult Index() {
        var user = GetDefaultUser();
        return View(user);
    }

    [HttpGet("api/isNameAvailable")]
    public async Task<bool> IsNameAvailable([FromQuery]string name) {
        return await userRepo.IsNameAvailableAsync(name);
    }

    [HttpPost("api/changeName")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeName([FromBody]string name) {
        var available = await userRepo.IsNameAvailableAsync(name);
        if (!available) {
            return UnprocessableEntity("Name is not available");
        }

        var res = await userRepo.ChangeNameAsync(GetUserId(), name);
        return res.Match<IActionResult>(
            Ok,
            err => {
                if (err is UserNotFoundException) {
                    return NotFound("User not found");
                }
                if (err is UserOperationException) {
                    return UnprocessableEntity();
                }

                return StatusCode(500);
            }
        );
    }

    private string GetUserId() {
        return HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException();
    }

    private UserModel GetDefaultUser() =>
        new UserModel() {
            Email = "foo@bar.de",
            UserName = "Patrick",
            EmailConfirmed = true,
            MFAEnabled = false,
            LockedOut = false,
            UserId = "6EA63E96-CF56-4E2F-8AC3-BE20C9798149",
            Roles = new[] {
                new RoleModel {RoleId = "40E89E74-6819-469C-8739-791D7BF675C6",RoleName = "IDENTITY_USER", RoleDescription = "The default user on the Identity network"},
                new RoleModel {RoleId = "7CECC4FA-56D3-4C78-A6C8-1101BA45D2F5",RoleName = "IDENTITY_ADMIN", RoleDescription = "The admin user on the Identity network"}
            }
        };
}