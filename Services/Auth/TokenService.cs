using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace GenericApi.Services.Auth
{
    public class TokenService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        private readonly AppDbContext _context = new();

        private readonly Serilog.ILogger _logger = Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                "refresh/log.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
            )
            .CreateLogger();

        public record TokenResult(string Token, List<string> Permissions);

        public TokenResult GenerateAccessToken(UserJwtDto user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? throw new Exception("JWT Key is missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresMinutes = double.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15");
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            // every time an access token is generated, get the roles and list of permissions for the user
            // Cache the user roles for the given user ID
            var userRoles = _context
                .UserRoles.Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Permission)
                .ToList();

            var roles = userRoles
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role.RoleName)
                .ToList();

            var permissions = userRoles
                .Where(ur => ur.Role != null && ur.Role.RoleModulePermissions != null)
                .SelectMany(ur => ur.Role.RoleModulePermissions)
                .Where(rmp => rmp.Permission != null && rmp.Permission.PermissionName != null)
                .Select(rmp => rmp.Permission.PermissionName)
                .Distinct()
                .ToList();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new("user", System.Text.Json.JsonSerializer.Serialize(user)),
            };

            // persist roles in the claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // persist permissions in the claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), permissions);
        }

        public string GenerateRefreshToken(int userId, string userAgent, string? ipAddress = null)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = Convert.ToBase64String(randomBytes);

            var jwtSettings = _configuration.GetSection("Jwt");
            var expiresDays = double.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");

            // save the refresh token in the database
            _context.RefreshTokens.Add(
                new RefreshToken
                {
                    Token = refreshToken,
                    // TODO: Change AddMinutes to AddDays for refresh token expiration after testing
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expiresDays),
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    UserAgent = userAgent,
                    IpAddress = ipAddress ?? "Unknown IP",
                }
            );

            _context.SaveChanges();

            return refreshToken;
        }

        public UserJwtDto GetUserFromAccessToken(string accessToken)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            if (token == null)
            {
                throw new Exception("Invalid access token");
            }

            var userClaim = token.Claims.FirstOrDefault(c => c.Type == "user");

            if (userClaim == null)
            {
                throw new Exception("User claim not found in access token");
            }

            return System.Text.Json.JsonSerializer.Deserialize<UserJwtDto>(userClaim.Value)
                ?? throw new Exception("Failed to deserialize user from access token");
        }

        public void DeleteExpiredRefreshTokens()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _context.RefreshTokens.Where(rt => rt.ExpiresAt < now).ToList();

            if (expiredTokens.Count != 0)
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
                _context.SaveChanges();
                _logger.Information(
                    "Deleted {Count} expired refresh tokens at {Time}",
                    expiredTokens.Count,
                    now
                );
            }
            else
            {
                _logger.Information("No expired refresh tokens to delete at {Time}", now);
            }
        }
    }
}
