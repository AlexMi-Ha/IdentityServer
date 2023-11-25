using IdentityServer.Core.Dto;
using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer.Data.Extensions; 

internal static class ApplicationUserExtensions {

    public static UserModel MapToDto(this ApplicationUser user, RoleModel[] roles) {
        return new UserModel {
            UserId = user.Id,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            LockedOut = user.LockoutEnd >= DateTimeOffset.UtcNow,
            UserName = user.UserName!,
            MFAEnabled = user.TwoFactorEnabled,
            Roles = roles
        };
    }
}