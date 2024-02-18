using IdentityServer.Core.Dto;

namespace IdentityServer.UI.Models; 

public class RoleDialogViewModel {
    
    public List<string> AllRoles { get; set; }
    public List<RoleModel> UserRoles { get; set; }
    
    public string UserId { get; set; }
}