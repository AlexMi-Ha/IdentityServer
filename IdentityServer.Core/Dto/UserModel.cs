namespace IdentityServer.Core.Dto; 

public class UserModel {
    
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    
    public bool MFAEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool LockedOut { get; set; }
    
    public RoleModel[] Roles { get; set; }
}