using IdentityServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data.Persistence; 

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt) {
    }
}