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
                    _context.Users.Add(account);
                }
            }
            _context.SaveChanges();
        }
    }
}
