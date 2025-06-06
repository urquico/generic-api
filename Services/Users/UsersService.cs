using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Dtos.UserManagement;
using GenericApi.Models;
using GenericApi.Services.ScriptTools;
using GenericApi.Utils;
using GenericApi.Utils.Auth;
using GenericApi.Utils.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GenericApi.Services.Users
{
    public class UsersService(IConfiguration configuration)
    {
        private readonly AppDbContext _context = new();
        private readonly ApiResponse _response = new(new HttpContextAccessor());
        private readonly IConfiguration _configuration = configuration;
        private readonly SqlRunner _sqlRunner = new();

        public async Task<IActionResult> CreateUser(SignupRequestDto createUser, int? userId)
        {
            // convert [1, 2] to "1,2" for SQL parameter
            var parsedRoles = string.Join(",", createUser.UserRoles.Select(r => r.ToString()));

            var email = new SqlParameter("@Email", createUser.Email);
            var password = new SqlParameter("@Password", createUser.Password);
            var confirmPassword = new SqlParameter("@ConfirmPassword", createUser.ConfirmPassword);
            var hashedPassword = new SqlParameter(
                "@HashedPassword",
                GenerateHashedPassword(createUser.Password)
            );
            var firstName = new SqlParameter("@FirstName", createUser.FirstName);
            var middleName = new SqlParameter("@MiddleName", createUser.MiddleName);
            var lastName = new SqlParameter("@LastName", createUser.LastName);
            var createdBy = new SqlParameter("@CreatedBy", userId ?? (object)DBNull.Value);
            var userRoleIds = new SqlParameter("@UserRoleIds", parsedRoles);

            return await _sqlRunner.RunStoredProcedureAsync<CreateUserResponseDto>(
                sqlQuery: "EXEC fmis.sp_create_user @Email, @Password, @ConfirmPassword, @HashedPassword, @FirstName, @MiddleName, @LastName, @CreatedBy, @UserRoleIds, @StatusCode OUTPUT, @Message OUTPUT, @Data OUTPUT",
                activity: string.Format(SignupMessages.ACTIVITY, createUser.Email),
                email,
                password,
                confirmPassword,
                hashedPassword,
                firstName,
                middleName,
                lastName,
                createdBy,
                userRoleIds
            );
        }

        public IActionResult ChangePassword(int userId, string password, string ip)
        {
            var hashedPassword = GenerateHashedPassword(password);

            // check if user exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    e: new Exception(UserMessages.USER_NOT_FOUND),
                    saveLog: true
                );
            }

            // update user password
            existingUser.Password = hashedPassword;
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = userId;
            _context.Update(existingUser);
            _context.SaveChanges();

            return _response.Success(
                statusCode: StatusCodes.Status200OK,
                activity: UserMessages.PASSWORD_CHANGED_SUCCESS,
                ip: ip,
                message: UserMessages.PASSWORD_CHANGED_SUCCESS,
                data: null
            );
        }

        public string GenerateHashedPassword(string password)
        {
            var saltRoundsStr = _configuration.GetSection("PasswordHashing")["SaltRounds"];
            int saltRounds = int.TryParse(saltRoundsStr, out var rounds) ? rounds : 12; // fallback to 12
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: saltRounds);
        }

        public IActionResult UpdateUser<T>(int userId, T user, string ip)
        {
            // check if user exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    e: new Exception(UpdateUserMessages.NOT_FOUND),
                    saveLog: true
                );
            }

            // update user information
            if (user != null)
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(user);
                    if (value != null)
                    {
                        var existingProp = existingUser.GetType().GetProperty(prop.Name);
                        if (existingProp != null && existingProp.CanWrite)
                        {
                            existingProp.SetValue(existingUser, value);
                        }
                    }
                }
            }

            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = userId;

            _context.Update(existingUser);
            _context.SaveChanges();

            return _response.Success(
                statusCode: StatusCodes.Status200OK,
                activity: string.Format(UpdateUserMessages.ACTIVITY, existingUser.Email),
                ip: ip,
                message: UpdateUserMessages.SUCCESS,
                data: null
            );
        }

        public IActionResult GetUserById(int userId)
        {
            // get user by its userId
            var user = _context
                .Users.Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.MiddleName,
                    u.LastName,
                    // get status name
                    Status = u.Status != null ? u.Status.CategoryValue : null,

                    // Get all of its roles based from the UserRoles table and get the role name from the Roles table
                    Roles = u.UserRoles.Select(ur => new
                    {
                        ur.RoleId,
                        RoleName = _context
                            .Roles.Where(r => r.Id == ur.RoleId)
                            .Select(r => r.RoleName)
                            .FirstOrDefault(),
                    }),
                })
                .FirstOrDefault();

            // check if user exists
            if (user == null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    e: new Exception(GetUserMessages.NOT_FOUND),
                    saveLog: true
                );
            }

            return _response.Success(
                statusCode: StatusCodes.Status200OK,
                message: GetUserMessages.SUCCESS,
                data: user
            );
        }

        public IActionResult CreateSpecialPermission(
            int userId,
            SpecialPermissionRequestDto specialPermission,
            int loggedUser,
            string ip,
            bool accessStatus
        )
        {
            // check if the user exists
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    e: new Exception(UserSpecialPermissionMessages.USER_NOT_FOUND)
                );
            }

            // check if the permission exists
            var permission = _context.ModulePermissions.FirstOrDefault(p =>
                p.Id == specialPermission.PermissionId
            );
            if (permission == null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status404NotFound,
                    e: new Exception(UserSpecialPermissionMessages.PERMISSION_NOT_FOUND)
                );
            }

            // check if the user already has the permission
            var existingPermission = _context.UserSpecialPermissions.FirstOrDefault(usp =>
                usp.UserId == userId && usp.PermissionId == specialPermission.PermissionId
            );
            if (existingPermission != null)
            {
                return _response.Error(
                    statusCode: StatusCodes.Status400BadRequest,
                    e: new Exception(
                        string.Format(
                            UserSpecialPermissionMessages.PERMISSION_ALREADY_EXISTS,
                            permission.PermissionName
                        )
                    )
                );
            }

            // grant the permission to the user
            var userPermission = new UserSpecialPermission
            {
                UserId = userId,
                PermissionId = specialPermission.PermissionId,
                AccessStatus = accessStatus,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = loggedUser,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = loggedUser,
            };
            _context.UserSpecialPermissions.Add(userPermission);
            _context.SaveChanges();

            return _response.Success(
                statusCode: StatusCodes.Status200OK,
                activity: accessStatus
                    ? string.Format(
                        UserSpecialPermissionMessages.ACTIVITY_GRANTED,
                        user.Email,
                        permission.PermissionName
                    )
                    : string.Format(
                        UserSpecialPermissionMessages.ACTIVITY_REVOKED,
                        user.Email,
                        permission.PermissionName
                    ),
                ip: ip,
                message: UserSpecialPermissionMessages.SUCCESS,
                data: null
            );
        }
    }
}
