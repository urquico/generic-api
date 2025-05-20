using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScaffoldTest.Dtos;
using ScaffoldTest.Dtos.Users;
using ScaffoldTest.Models;
using ScaffoldTest.Utils;

namespace EfScaffoldDemo.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    public class UsersController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly CustomSuccess _response = new();

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            try
            {
                var user = await _context
                    .Users.FromSqlRaw(
                        "EXEC mwss.AddUser @Username = {0}, @Email = {1}",
                        request.Username,
                        request.Email
                    )
                    .ToListAsync();

                var userEntity = user.FirstOrDefault();

                AddUserResponse? responseDto = null;

                if (userEntity != null)
                {
                    responseDto = new AddUserResponse
                    {
                        Id = userEntity.Id,
                        Username = userEntity.Username!,
                        Email = userEntity.Email!,
                        Birthday = userEntity.Birthday?.ToString("MMM dd yyyy"),
                        Created_at = userEntity.CreatedAt.HasValue
                            ? TimeZoneInfo
                                .ConvertTime(
                                    userEntity.CreatedAt.Value,
                                    TimeZoneInfo.Utc,
                                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
                                )
                                .ToString("MMM dd yyyy HH:mm")
                            : null,
                        Is_active = userEntity.IsActive,
                    };
                }

                const string activity = "User has been added.";
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 201,
                    activity: activity,
                    ip: ip,
                    message: "User added successfully.",
                    data: responseDto
                );
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return _response.Error(statusCode: 500, e: ex);
            }
        }

        // get single user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            try
            {
                var user = await _context
                    .VwUserByUsernameEmails.Where(v => v.Id == id)
                    .ToListAsync();

                if (user == null || !user.Any())
                {
                    return _response.Error(statusCode: 404, e: new Exception("User not found."));
                }

                var responseDto = new GetSingleUserResponse
                {
                    Id = user.First().Id,
                    Username = user.First().Username!,
                    Email = user.First().Email!,
                    Birthday = user.First().Birthday?.ToString("MMM dd yyyy"),
                    Created_at = user.First().CreatedAt is DateTime createdAt
                        ? TimeZoneInfo
                            .ConvertTime(
                                createdAt,
                                TimeZoneInfo.Utc,
                                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
                            )
                            .ToString("MMM dd yyyy HH:mm")
                        : null,
                    Is_active = user.First().IsActive,
                };

                const string activity = "User has been retrieved.";
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 200,
                    activity: activity,
                    ip: ip,
                    message: "User retrieved successfully.",
                    data: responseDto
                );
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return _response.Error(statusCode: 500, e: ex);
            }
        }
    }
}
