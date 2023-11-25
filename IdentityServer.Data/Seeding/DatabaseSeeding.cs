using IdentityServer.Core.Dto;
using IdentityServer.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Data.Seeding; 

internal class DatabaseSeeding {

    private readonly IUserRepository _userRepo;

    private readonly string[] _firstNames = {
        "Emilia", 	
        "Noah",
        "Mia",
        "Matteo",
        "Sophia",
        "Elias",
        "Emma",
        "Finn",
        "Hannah",
        "Leon",
        "Lina",
        "Theo",
        "Mila",
        "Paul",
        "Ella",
        "Emil",
        "Leni",
        "Henry",
        "Clara",
        "Ben", 
    };

    private readonly string[] _lastNames = {
        "Müller",
        "Schmidt",
        "Schneider",
        "Fischer",
        "Weber",
        "Meyer",
        "Wagner",
        "Becker",
        "Schulz",
        "Hoffmann",
        "Schäfer",
        "Koch",
        "Bauer",
        "Richter",
        "Klein",
        "Wolf",
        "Schröder",
        "Neumann",
        "Schwarz",
        "Zimmermann",
    };

    private readonly ILogger<DatabaseSeeding> _logger;

    private readonly IRoleRepository _roleRepo;
    
    private readonly Random _rng;

    private static uint _mailCounter = 0;

    public DatabaseSeeding(IUserRepository userRepo, IRoleRepository roleRepo, ILogger<DatabaseSeeding> logger) 
        : this(userRepo, roleRepo, logger, 420){
        
    }

    public DatabaseSeeding(IUserRepository userRepo, IRoleRepository roleRepo, ILogger<DatabaseSeeding> logger, int seed) {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _logger = logger;
        _rng = new Random(seed);
    }

    public async Task SeedUsersAsync(int numUsers) {
        if (await _userRepo.AnyUsersAsync()) {
            return;
        }
        var created = 0;
        for (int i = 0; i < numUsers; ++i) {
            var name = GenerateRandomName();
            var model = new RegisterModel {
                Email = GenerateEmail(name),
                Name = name,
                Password = "Admin1!"
            };
            var res = await _userRepo.CreateUserAsync(model);
            if (res.IsFaulted) {
                continue;
            }
            ++created;
        }

        if (_logger.IsEnabled(LogLevel.Information)) {
            _logger.LogInformation("Created {created} users!", created);
        }

        await SeedRolesAsync();
    }

    private async Task SeedRolesAsync() {

        var res = await _roleRepo.AddNewRoleAsync(new RoleModel() {
            RoleDescription = "This is the Identity Server Admin Role",
            RoleName = "IDENTITY_ADMIN"
        });

        await _userRepo.CreateUserAsync(new RegisterModel() {
            Email = "seedtestlel+identity.admin@gmail.com",
            Name = "Admin",
            Password = "Admin1!"
        });

        var user = await _userRepo.GetUserByEmailAsync("seedtestlel+identity.admin@gmail.com");
        if (user.IsFaulted) {
            _logger.LogInformation("Failed to create admin user");
            return;
        }

        await _roleRepo.AddUserToRoleAsync(user.Value!.UserId, "IDENTITY_ADMIN");
    }
    
    private string GenerateRandomName() {
        var firstname = _firstNames[_rng.Next(_firstNames.Length)];
        var lastName = _lastNames[_rng.Next(_lastNames.Length)];
        return firstname + " " + lastName;
    }

    private string GenerateEmail(string name) {
        return $"seedtestlel+identity.{_mailCounter++}@gmail.com";
    }
}