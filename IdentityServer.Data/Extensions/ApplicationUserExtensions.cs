using IdentityServer.Core.Dto;
using IdentityServer.Data.Models;

namespace IdentityServer.Data.Extensions; 

public static class ApplicationUserExtensions {

    public static UserModel MapToDto(this ApplicationUser user) {
        return new UserModel {
            UserId = user.Id,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            LockedOut = user.LockoutEnd >= DateTimeOffset.UtcNow,
            UserName = user.UserName!,
            MFAEnabled = user.TwoFactorEnabled
        };
    }
}