using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Microsoft.AspNetCore.Components.Route("admin")]
[Authorize(Roles = "IDENTITY_ADMIN")]
public class AdminController : ControllerBase {

    public IActionResult Index() {
        return View();
    }

    public IActionResult UsersDashboard() {
        return NotFound();
    }

    public IActionResult RolesDashboard() {
        return NotFound();
    }
}