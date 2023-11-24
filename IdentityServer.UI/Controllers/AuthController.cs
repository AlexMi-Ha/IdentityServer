using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.UI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("auth")]
public class AuthController : Controller {
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _configuration;
    public AuthController(IUserRepository userRepo, IConfiguration configuration) {
        _userRepo = userRepo;
        _configuration = configuration;
    }

    [HttpGet("login")]
    public IActionResult Login([FromQuery]string? returnUrl) {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpGet("register")]
    public IActionResult Register([FromQuery]string? returnUrl) {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpGet("logout")]
    public IActionResult Logout([FromQuery]string? returnUrl) {
        HttpContext.Response.Cookies.Delete("identity-token");
        if (string.IsNullOrWhiteSpace(returnUrl)) {
            return RedirectToAction(nameof(Login));
        }

        return RedirectToRoute(returnUrl);
    }

    [HttpPost("loginaction")]
    public async Task<IActionResult> LoginAction([FromForm]LoginModel loginModel, [FromQuery]string? returnUrl) {
        var jwt = await _userRepo.LoginUserAsync(loginModel);
        return jwt.Match<IActionResult>(
            succ => {
                SetupAuthCookie(succ);
                if (string.IsNullOrWhiteSpace(returnUrl)) {
                    return RedirectToAction("Index", "User");
                }
                return RedirectToRoute(returnUrl);
            },
            err => {
                return err switch {
                    ValidationException => UnprocessableEntity(err.Message),
                    AuthFailureException => Unauthorized(err.Message),
                    _ => StatusCode(500)
                };
            }
        );
    }

    [HttpPost("registeraction")]
    public async Task<IActionResult> RegisterAction([FromForm] RegisterModel registerModel,
        [FromQuery] string? returnUrl) {

        var jwt = await _userRepo.RegisterUserAsync(registerModel);
        return jwt.Match<IActionResult>(
            succ => {
                SetupAuthCookie(succ);
                if (string.IsNullOrWhiteSpace(returnUrl)) {
                    return RedirectToAction("Index", "User");
                }
                return RedirectToRoute(returnUrl);
            },
            err => {
                return err switch {
                    ValidationException => UnprocessableEntity(err.Message),
                    UserOperationException => UnprocessableEntity(err.Message),
                    AuthFailureException => Unauthorized(err.Message),
                    _ => StatusCode(500)
                };
            }
        );
    }

    private void SetupAuthCookie(JwtModel jwt) {
        HttpContext.Response.Cookies.Append(
            "identity-token",
            jwt.Token, 
                new CookieOptions(){
                    Domain = _configuration.GetCookieDomain(),
                    Expires = jwt.ExpiresDateTimeOffset,
                    Path = "/",
                    Secure = true,
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict
                }
            );
    }
}