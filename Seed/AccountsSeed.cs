using System.Text.Json;
using GenericApi.Models;
using Microsoft.Extensions.Configuration;

namespace GenericApi.Seed
{
    public class AccountsSeed(AppDbContext context, IConfiguration configuration)
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public void Seed()
        {
            var accountsJson = File.ReadAllText("Seed/Accounts.json");
            var accounts = JsonSerializer.Deserialize<List<User>>(accountsJson) ?? [];

            var saltRoundsStr = _configuration.GetSection("PasswordHashing")["SaltRounds"];
            int saltRounds = int.TryParse(saltRoundsStr, out var rounds) ? rounds : 12; // fallback to 12

            foreach (var account in accounts)
            {
                if (!_context.Users.Any(u => u.Email == account.Email))
                {
                    account.Password = BCrypt.Net.BCrypt.HashPassword(
                        account.Password,
                        workFactor: saltRounds
                    );
                    account.StatusId = 1;
                    account.CreatedAt = DateTime.UtcNow;
                    account.UpdatedAt = DateTime.UtcNow;

                    // Ensure "Super Admin" role exists with all permissions
                    var superAdminRole = _context.Roles.FirstOrDefault(r =>
                        r.RoleName == "Super Admin"
                    );

                    if (superAdminRole == null)
                    {
                        // get all module permissions
                        var modulePermissions = _context.ModulePermissions.ToList();

                        superAdminRole = new Role
                        {
                            RoleName = "Super Admin",
                            RoleStatus = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                        _context.Roles.Add(superAdminRole);
                        _context.SaveChanges();

                        // Now create RoleModulePermissions after superAdminRole.Id is generated
                        var roleModulePermissions = _context
                            .ModulePermissions.Select(mp => new RoleModulePermission
                            {
                                RoleId = superAdminRole.Id,
                                PermissionId = mp.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                            })
                            .ToList();

                        superAdminRole.RoleModulePermissions = roleModulePermissions;
                        _context.SaveChanges();
                    }

                    // Assign "Super Admin" role to the user
                    account.UserRoles = new List<UserRole>
                    {
                        new()
                        {
                            RoleId = superAdminRole.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        },
                    };
                    _context.Users.Add(account);
                }
            }
            _context.SaveChanges();
        }
    }
}
