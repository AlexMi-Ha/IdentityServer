using IdentityServer.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("usercontent")]
public class UserContentController(IUserImageRepository _userImageRepo) : Controller {

    [HttpGet("{userId}")]
    public IActionResult GetUserImageUrl(string userId) {
        var path = _userImageRepo.GetImagePathForUser(userId);
        return Redirect(path);
    }
}