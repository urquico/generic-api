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

        public string GenerateAccessToken(UserJwtDto user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? throw new Exception("JWT Key is missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresMinutes = double.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15");
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            // TODO: Every time an access token is generated, get the roles and list of permissions for the user from the database and add them to the claims.

            var userDto = new UserJwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                // Add other properties as needed
            };

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new("user", System.Text.Json.JsonSerializer.Serialize(userDto)),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
