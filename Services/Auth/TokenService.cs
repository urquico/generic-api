using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Models;
using GenericApi.Services.ScriptTools;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using static GenericApi.Services.ScriptTools.SqlRunner;

namespace GenericApi.Services.Auth
{
    public class TokenService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        private readonly AppDbContext _context = new();
        private readonly SqlRunner _sqlRunner = new();

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

            var roles = user.Roles.Select(r => r.RoleName).ToList();
            var permissions = user.Permissions.Select(p => p.PermissionName).ToList();

            var minimalUser = new MinimalUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
            };

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new("user", JsonSerializer.Serialize(minimalUser)),
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

        public async Task<string> GenerateRefreshToken(
            int userId,
            string userAgent,
            string? ipAddress = null
        )
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = Convert.ToBase64String(randomBytes);

            var jwtSettings = _configuration.GetSection("Jwt");
            var expiresDays = double.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");

            var userIdParam = new SqlParameter("@UserId", userId);
            var tokenParam = new SqlParameter("@Token", refreshToken);
            var expiresAtParam = new SqlParameter(
                "@ExpiresAt",
                DateTime.UtcNow.AddDays(expiresDays)
            );
            var userAgentParam = new SqlParameter("@UserAgent", userAgent);
            var ipAddressParam = new SqlParameter("@IpAddress", ipAddress ?? "Unknown IP");
            var createdByParam = new SqlParameter("@CreatedBy", userId.ToString());

            // save the refresh token in the database
            await _sqlRunner.RunStoredProcedureRaw<object>(
                sqlQuery: "EXEC fmis.sp_refresh_token_create @UserId, @Token, @ExpiresAt, @UserAgent, @IpAddress, @CreatedBy, @StatusCode OUTPUT, @Message OUTPUT, @Data OUTPUT",
                userIdParam,
                tokenParam,
                expiresAtParam,
                userAgentParam,
                ipAddressParam,
                createdByParam
            );

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

            return JsonSerializer.Deserialize<UserJwtDto>(userClaim.Value)
                ?? throw new Exception("Failed to deserialize user from access token");
        }

        public async Task<
            StoredProcedureRawResult<ValidateRefreshTokenResponseDto>
        > CheckRefreshTokenValidity(string refreshToken)
        {
            var tokenParam = new SqlParameter("@Token", refreshToken);

            var token = await _sqlRunner.RunStoredProcedureRaw<ValidateRefreshTokenResponseDto>(
                sqlQuery: "EXEC fmis.sp_refresh_token_validate @Token, @StatusCode OUTPUT, @Message OUTPUT, @Data OUTPUT",
                tokenParam
            );

            return token;
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
