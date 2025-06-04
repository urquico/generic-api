using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos;
using GenericApi.Dtos.UserManagement;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Services.Users;
using GenericApi.Utils;
using GenericApi.Utils.SwaggerSummary;
using GenericApi.Utils.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/users")]
    [SwaggerTag("Users Management")]
    public class UserManagementController(
        UsersService usersService,
        TokenService tokenService,
        IConfiguration configuration
    ) : ControllerBase
    {
        private readonly CustomSuccess _response = new();
        private readonly UsersService _usersService = usersService;
        private readonly TokenService _tokenService = tokenService;
        private readonly AppDbContext _context = new();
        private readonly IConfiguration _configuration = configuration;

        /**
         * Get all users endpoint allows fetching all users with optional filters.
         *
         * @param getAllUsersDto The request body containing user filters.
         * @returns {IActionResult} 200 if fetching is successful, 500 if an error occurred.
         * @route GET /all
        */
        [HttpGet("all")]
        [PermissionAuthorize("Admin.GetAllUsers")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.GET_ALL_USERS)]
        public IActionResult GetAllUsers([FromQuery] GetAllUsersQueryDto query)
        {
            try
            {
                // get all users from the database with optional filters
                var usersQuery = _context.Users.AsQueryable();

                if (query.includeDeleted == 1)
                {
                    usersQuery = usersQuery.Where(u => u.DeletedAt != null || u.DeletedAt == null);
                }
                else
                {
                    usersQuery = usersQuery.Where(u => u.DeletedAt == null);
                }

                var users = usersQuery
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.CreatedAt,
                        u.UpdatedAt,
                        Status = u.Status != null ? u.Status.CategoryValue : null,
                        Roles = u
                            .UserRoles.Select(ur => ur.Role != null ? ur.Role.RoleName : null)
                            .ToList(),
                        // get last login on the last refresh token
                        LastLogin = _context
                            .RefreshTokens.Where(rt => rt.UserId == u.Id)
                            .OrderByDescending(rt => rt.CreatedAt)
                            .Select(rt => rt.CreatedAt)
                            .FirstOrDefault(),
                    })
                    .ToList();

                var parsedUsers = users
                    .Select(u => new GetAllUsersResponseDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FullName = $"{u.FirstName} {u.LastName}",
                        Status = u.Status ?? "Unknown",
                        Roles = u.Roles.Where(r => r != null).Select(r => r!).ToArray(),
                        DateCreated =
                            u.CreatedAt != null
                                ? u.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                : "",
                        LastLogin = u.LastLogin.HasValue
                            ? u.LastLogin.Value.ToString("yyyy-MM-dd HH:mm:ss")
                            : "Never",
                    })
                    .ToArray();
                var response = new PaginationResponseDto<GetAllUsersResponseDto[]>
                {
                    Items = [parsedUsers],
                    TotalCount = users.Count,
                    CurrentPage = 0,
                    TotalPages = 1,
                };

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    message: "Users fetched successfully.",
                    data: response
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Get user by ID endpoint allows fetching a user by their ID.
         *
         * @param id The ID of the user to fetch.
         * @returns {IActionResult} 200 if fetching is successful, 500 if an error occurred.
         * @route GET /{userId}
        */
        [HttpGet("{userId}")]
        [PermissionAuthorize("Admin.CanGetUserById")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.GET_SINGLE_USERS)]
        public IActionResult GetUserById([FromRoute] int userId)
        {
            try
            {
                return _usersService.GetUserById(userId);
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Create user endpoint allows creating a new user.
         *
         * @param createUserDto The request body containing user data.
         * @returns {IActionResult} 200 if creation is successful, 500 if an error occurred.
         * @route POST /
        */
        [HttpPost]
        [PermissionAuthorize("Admin.CanCreateUser")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.CREATE_USER)]
        public IActionResult CreateUser([FromBody] CreateUserRequestDto createUserDto)
        {
            try
            {
                var accessToken = HttpContext.Request.Cookies["accessToken"] ?? "";
                var loggedUser = _tokenService.GetUserFromAccessToken(accessToken);
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _usersService.CreateUser(
                    createUser: createUserDto,
                    userId: loggedUser.Id,
                    ip: ip
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Update user by ID endpoint allows updating a user by their ID.
         *
         * @param userId The ID of the user to update.
         * @param updateUserDto The request body containing user update data.
         * @returns {IActionResult} 200 if updating is successful, 500 if an error occurred.
         * @route PUT /{userId}
        */
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.UPDATE_USER)]
        public IActionResult UpdateUserById(
            [FromRoute] int userId,
            [FromBody] UpdateUserRequestDto updateUserDto
        )
        {
            try
            {
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _usersService.UpdateUser(userId: userId, user: updateUserDto, ip: ip);
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Soft delete user by ID endpoint allows soft deleting a user by their ID.
         *
         * @param userId The ID of the user to soft delete.
         * @returns {IActionResult} 200 if soft deletion is successful, 500 if an error occurred.
         * @route DELETE /{userId}
        */
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.SOFT_DELETE_USER)]
        public IActionResult SoftDeleteUserById([FromRoute] int userId)
        {
            try
            {
                // check if the user exists
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(SoftDeletedMessages.USER_NOT_FOUND)
                    );
                }

                var loggedUser = _tokenService.GetUserFromAccessToken(
                    HttpContext.Request.Cookies["accessToken"] ?? ""
                );

                // soft delete the user
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = loggedUser?.Id ?? 0;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = loggedUser?.Id ?? 0;
                _context.Users.Update(user);
                _context.SaveChanges();

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: string.Format(SoftDeletedMessages.ACTIVITY, user.Email),
                    ip: ip,
                    message: SoftDeletedMessages.SUCCESS,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Restore user by ID endpoint allows restoring a soft-deleted user by their ID.
         *
         * @param userId The ID of the user to restore.
         * @returns {IActionResult} 200 if restoration is successful, 500 if an error occurred.
         * @route PATCH /{userId}/restore
         * @example response - 200 - User restored successfully
        */
        [HttpPatch("{userId}/restore")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.RESTORE_USER)]
        public IActionResult RestoreUserById([FromRoute] int userId)
        {
            try
            {
                // check if the user exists
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null || user.DeletedAt == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(RestoreUserMessages.USER_NOT_FOUND)
                    );
                }

                var loggedUser = _tokenService.GetUserFromAccessToken(
                    HttpContext.Request.Cookies["accessToken"] ?? ""
                );

                // restore the user
                user.DeletedAt = null;
                user.DeletedBy = null;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = loggedUser?.Id ?? 0;
                _context.Users.Update(user);
                _context.SaveChanges();

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: string.Format(RestoreUserMessages.ACTIVITY, user.Email),
                    ip: ip,
                    message: RestoreUserMessages.SUCCESS,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Force delete user by ID endpoint allows force deleting a user by their ID.
         *
         * @param userId The ID of the user to force delete.
         * @returns {IActionResult} 200 if force deletion is successful, 500 if an error occurred.
         * @route DELETE /{userId}/force
         * @example response - 200 - User force deleted successfully
         * {
         *   "statusCode": 200,
         *   "message": "User force deleted successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while force deleting the user."
         * }
        */
        [HttpDelete("{userId}/force")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Force delete user by ID.")]
        public IActionResult ForceDeleteUserById([FromRoute] string userId)
        {
            try
            {
                // TODO: Implement the logic for force deleting user by ID

                const string activity = "User force deleted successfully.";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 200,
                    activity: activity,
                    ip: ip,
                    message: activity,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: 500, e: ex);
            }
        }

        /**
         * Update user status by ID endpoint allows updating the status of a user by their ID.
         *
         * @param userId The ID of the user to update status.
         * @param updateUserStatusDto The request body containing user status update data.
         * @returns {IActionResult} 200 if updating is successful, 500 if an error occurred.
         * @route PATCH /{userId}/status
        */
        [HttpPatch("{userId}/status")]
        [PermissionAuthorize("Admin.CanUpdateUserStatus")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Update user status by ID.")]
        public IActionResult UpdateUserStatusById(
            [FromRoute] int userId,
            [FromBody] UpdateUserStatusRequestDto updateUserStatusDto
        )
        {
            try
            {
                // update user StatusId by updateUserStatusDto.Status
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(UpdateUserStatusMessages.USER_NOT_FOUND)
                    );
                }

                // check if the status exists
                var status = _context.KeyCategories.FirstOrDefault(s =>
                    s.CategoryId == updateUserStatusDto.Status
                    && s.CategoryName == AppCategoryNames.USER_STATUS
                );
                if (status == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(UpdateUserStatusMessages.STATUS_NOT_FOUND)
                    );
                }

                // update user status
                user.StatusId = status.Id;
                user.UpdatedAt = DateTime.UtcNow;
                var accessToken = HttpContext.Request.Cookies["accessToken"] ?? "";
                var loggedUser = _tokenService.GetUserFromAccessToken(accessToken);
                user.UpdatedBy = loggedUser?.Id ?? 0;
                _context.Users.Update(user);
                _context.SaveChanges();

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: string.Format(
                        UpdateUserStatusMessages.ACTIVITY,
                        user.Email,
                        status.CategoryValue
                    ),
                    ip: ip,
                    message: UpdateUserStatusMessages.SUCCESS,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Reset user password by ID endpoint allows resetting the password of a user by their ID.
         *
         * @param userId The ID of the user to reset password.
         * @param resetPasswordDto The request body containing new password data.
         * @returns {IActionResult} 200 if resetting is successful, 500 if an error occurred.
         * @route PATCH /{userId}/reset
        */
        [HttpPatch("{userId}/reset")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.RESET_PASSWORD)]
        public IActionResult ResetUserPasswordById([FromRoute] int userId)
        {
            try
            {
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
                string defaultPassword = _configuration["DefaultPassword"] ?? "";
                if (string.IsNullOrEmpty(defaultPassword) || defaultPassword == "")
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status400BadRequest,
                        e: new Exception(ResetPasswordMessages.NO_DEFAULT_PASSWORD),
                        saveLog: true
                    );
                }

                return _usersService.ChangePassword(
                    userId: userId,
                    password: defaultPassword,
                    ip: ip
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Block user access by ID endpoint allows blocking a user's access to a specific permission.
         *
         * @param userId The ID of the user to block access.
         * @param blockAccessDto The request body containing access block data.
         * @returns {IActionResult} 200 if blocking is successful, 500 if an error occurred.
         * @route POST /{userId}/access/block
         * @example response - 200 - User access blocked successfully
         * {
         *   "statusCode": 200,
         *   "message": "User access blocked successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while blocking user access."
         * }
        */
        [HttpPost("{userId}/access/block")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Block user access by ID.")]
        public IActionResult BlockUserAccessById(
            [FromRoute] string userId,
            [FromBody] BlockUserAccessRequestDto[] blockAccessDto
        )
        {
            try
            {
                // TODO: Implement the logic for blocking user access by ID

                const string activity = "User access blocked successfully.";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 200,
                    activity: activity,
                    ip: ip,
                    message: activity,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: 500, e: ex);
            }
        }

        /**
         * Grant user access by ID endpoint allows granting a user's access to a specific permission.
         *
         * @param userId The ID of the user to grant access.
         * @param grantAccessDto The request body containing access grant data.
         * @returns {IActionResult} 200 if granting is successful, 500 if an error occurred.
         * @route POST /{userId}/access/permission
         * @example response - 200 - User access granted successfully
         * {
         *   "statusCode": 200,
         *   "message": "User access granted successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while granting user access."
         * }
        */
        [HttpPost("{userId}/access/permission")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Grant user access by ID.")]
        public IActionResult GrantUserAccessById(
            [FromRoute] string userId,
            [FromBody] GrantUserAccessRequestDto[] grantAccessDto
        )
        {
            try
            {
                // TODO: Implement the logic for granting user access by ID

                const string activity = "User access granted successfully.";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 200,
                    activity: activity,
                    ip: ip,
                    message: activity,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: 500, e: ex);
            }
        }
    }
}
