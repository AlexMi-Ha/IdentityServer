using System.Security.Claims;
using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Authorize]
[Route("user")]
public class UserController : Controller {

    private readonly IUserRepository _userRepo;
    private readonly IUserOperationService _userOpService;
    public UserController(IUserRepository userRepo, IUserOperationService userOpService) {
        _userOpService = userOpService;
        _userRepo = userRepo;
    }

    public async Task<IActionResult> Index() {
        var userId = GetUserId();
        var user = await _userRepo.GetUserByIdAsync(userId);
        
        return user.Match<IActionResult>(
            View,
            err => err switch {
                UserNotFoundException => NotFound("User not found"),
                _ => StatusCode(500)
            }
        );
    }

    [HttpGet("api/isNameAvailable")]
    public async Task<bool> IsNameAvailable([FromQuery]string name) {
        return await _userOpService.IsNameAvailableAsync(name);
    }

    [HttpPost("api/changeName")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeName([FromBody]string name) {
        var available = await _userOpService.IsNameAvailableAsync(name);
        if (!available) {
            return UnprocessableEntity("Name is not available");
        }

        var res = await _userOpService.ChangeNameAsync(new ChangeNameModel(GetUserId(), name));
        return res.Match<IActionResult>(
            Ok,
            err => {
                return err switch {
                    ValidationException => UnprocessableEntity(err.Message),
                    UserNotFoundException => NotFound("User not found"),
                    UserOperationException => UnprocessableEntity(),
                    _ => StatusCode(500)
                };
            }
        );
    }

    private string GetUserId() {
        return HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException();
    }
}