using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Models;
using GenericApi.Utils;
using GenericApi.Utils.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi.Services.Users
{
    public class UsersService(IConfiguration configuration)
    {
        private readonly AppDbContext _context = new();
        private readonly CustomSuccess _response = new();
        private readonly IConfiguration _configuration = configuration;

        public IActionResult CreateUser(SignupRequestDto createUser, int? userId)
        {
            // Check if the email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == createUser.Email);
            if (existingUser != null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status400BadRequest,
                    e: new Exception(SignupMessages.EMAIL_ALREADY_EXISTS),
                    saveLog: true
                );
            }

            // Check if password and confirm password match
            if (createUser.Password != createUser.ConfirmPassword)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status400BadRequest,
                    e: new Exception(SignupMessages.PASSWORD_CONFIRMATION_MISMATCH),
                    saveLog: true
                );
            }

            var saltRoundsStr = _configuration.GetSection("PasswordHashing")["SaltRounds"];
            int saltRounds = int.TryParse(saltRoundsStr, out var rounds) ? rounds : 12; // fallback to 12
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                createUser.Password,
                workFactor: saltRounds
            );

            // save the new user to the database
            var insertedUser = _context.Users.Add(
                new User
                {
                    Email = createUser.Email,
                    Password = hashedPassword,
                    FirstName = createUser.FirstName,
                    MiddleName = createUser.MiddleName,
                    LastName = createUser.LastName,
                    StatusId = 1,
                    UserRoles =
                    [
                        .. createUser.UserRoles.Select(roleId => new UserRole
                        {
                            RoleId = roleId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = userId,
                            UpdatedAt = DateTime.UtcNow,
                        }),
                    ],
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                }
            );

            _context.SaveChanges();

            return _response.Success(
                statusCode: StatusCodes.Status201Created,
                message: "User created successfully.",
                data: new
                {
                    insertedUser.Entity.Id,
                    insertedUser.Entity.Email,
                    insertedUser.Entity.FirstName,
                    insertedUser.Entity.LastName,
                    // Add other fields as needed
                }
            );
        }
    }
}
