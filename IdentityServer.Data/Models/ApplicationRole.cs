using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Data.Models; 

public class ApplicationRole(string name, string roleDescription)
    : IdentityRole(name) {
    public string RoleDescription { get; set; } = roleDescription;
}