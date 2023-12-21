using System.Security.Claims;
using IdentityServer.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.UI.Controllers; 

public class ControllerBase : Controller {
    protected string GetUserId() {
        return HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException();
    }
}