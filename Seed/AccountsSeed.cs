using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GenericApi.Models;

namespace GenericApi.Seed
{
    public class AccountsSeed(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public void Seed()
        {
            var accountsJson = File.ReadAllText("Seed/Accounts.json");
            var accounts = JsonSerializer.Deserialize<List<User>>(accountsJson) ?? [];

            foreach (var account in accounts)
            {
                if (!_context.Users.Any(u => u.Email == account.Email))
                {
                    account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
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
