using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data.Persistence; 

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string> {

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt) {
    }
}