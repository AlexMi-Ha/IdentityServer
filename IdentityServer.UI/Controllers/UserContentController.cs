using IdentityServer.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("usercontent")]
public class UserContentController : Controller {
    private readonly IUserImageRepository _userImageRepo1;
    public UserContentController(IUserImageRepository _userImageRepo) {
        _userImageRepo1 = _userImageRepo;
    }

    [HttpGet("{userId}")]
    public IActionResult GetUserImageUrl(string userId) {
        var path = _userImageRepo1.GetImagePathForUser(userId);
        return Redirect(path);
    }
}