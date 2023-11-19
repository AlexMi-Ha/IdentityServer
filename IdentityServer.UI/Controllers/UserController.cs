using IdentityServer.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("user")]
public class UserController : Controller {
    
    public IActionResult Index() {
        var user = GetDefaultUser();
        return View(user);
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