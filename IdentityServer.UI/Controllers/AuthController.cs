﻿using IdentityServer.Core.Dto;
using IdentityServer.Core.Exceptions;
using IdentityServer.Core.Interfaces.Repositories;
using IdentityServer.UI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

[Route("auth")]
public class AuthController(IUserRepository userRepo, IConfiguration configuration) : Controller {
    
    [HttpGet("login")]
    public IActionResult Login() {
        return View();
    }

    [HttpGet("register")]
    public IActionResult Register() {
        return View();
    }

    [HttpPost("loginaction")]
    public async Task<IActionResult> LoginAction([FromForm]LoginModel loginModel, [FromQuery]string? returnUrl) {
        var jwt = await userRepo.LoginUserAsync(loginModel);
        return jwt.Match<IActionResult>(
            succ => {
                SetupAuthCookie(succ);
                if (string.IsNullOrWhiteSpace(returnUrl)) {
                    return RedirectToAction("Profile", "User");
                }
                return RedirectToRoute(returnUrl);
            },
            err => {
                if (err is AuthFailureException afx) {
                    return Unauthorized(afx.Message);
                }
                return StatusCode(500);
            }
        );
    }

    [HttpPost("registeraction")]
    public async Task<IActionResult> RegisterAction([FromForm] RegisterModel registerModel,
        [FromQuery] string? returnUrl) {

        var jwt = await userRepo.RegisterUserAsync(registerModel);
        return jwt.Match<IActionResult>(
            succ => {
                SetupAuthCookie(succ);
                if (string.IsNullOrWhiteSpace(returnUrl)) {
                    return RedirectToAction("Profile", "User");
                }
                return RedirectToRoute(returnUrl);
            },
            err => {
                if (err is AuthFailureException afx) {
                    return Unauthorized(afx.Message);
                }else if (err is UserOperationException uoe) {
                    return UnprocessableEntity(uoe.Message);
                }
                return StatusCode(500);
            }
        );
    }

    private void SetupAuthCookie(JwtModel jwt) {
        HttpContext.Response.Cookies.Append(
            "token",
            jwt.Token, 
                new CookieOptions(){
                    Domain = configuration.GetCookieDomain(),
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