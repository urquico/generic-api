using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.UserManagement;
using GenericApi.Services.Auth;
using GenericApi.Services.Users;
using GenericApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/users")]
    [SwaggerTag("Users Management")]
    public class UserManagementController(UsersService usersService, TokenService tokenService)
        : ControllerBase
    {
        private readonly CustomSuccess _response = new();
        private readonly UsersService _usersService = usersService;
        private readonly TokenService _tokenService = tokenService;

        /**
         * Get all users endpoint allows fetching all users with optional filters.
         *
         * @param getAllUsersDto The request body containing user filters.
         * @returns {IActionResult} 200 if fetching is successful, 500 if an error occurred.
         * @route GET /all
         * @example response - 200 - Users fetched successfully
         * {
         *   "statusCode": 200,
         *   "message": "Users fetched successfully.",
         *   "data": [PaginatedUsers]
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while fetching users."
         * }
        */
        [HttpGet("all")]
        [PermissionAuthorize("UserManagement.GetAllUsers")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Get all users with optional filters.")]
        public IActionResult GetAllUsers([FromQuery] GetAllUsersQueryDto getAllUsersDto)
        {
            try
            {
                // TODO: Implement the logic for user logout

                const string activity = "Users fetched successfully.";
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
        [SwaggerOperation(Summary = "Get user by ID.")]
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
        [SwaggerOperation(Summary = "Create a new user.")]
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
         * @example response - 200 - User updated successfully
         * {
         *   "statusCode": 200,
         *   "message": "User updated
         * }
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while updating the user."
         * }
        */
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Update user by ID.")]
        public IActionResult UpdateUserById(
            [FromRoute] string userId,
            [FromBody] UpdateUserRequestDto updateUserDto
        )
        {
            try
            {
                // TODO: Implement the logic for updating user by ID

                const string activity = "User updated successfully.";
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
         * Soft delete user by ID endpoint allows soft deleting a user by their ID.
         *
         * @param userId The ID of the user to soft delete.
         * @returns {IActionResult} 200 if soft deletion is successful, 500 if an error occurred.
         * @route DELETE /{userId}
         * @example response - 200 - User soft deleted successfully
         * {
         *   "statusCode": 200,
         *   "message": "User soft deleted successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while soft deleting the user."
         * }
        */
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Soft delete user by ID.")]
        public IActionResult SoftDeleteUserById([FromRoute] string userId)
        {
            try
            {
                // TODO: Implement the logic for soft deleting user by ID

                const string activity = "User soft deleted successfully.";
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
         * @example response - 200 - User status updated successfully
         * {
         *   "statusCode": 200,
         *   "message": "User status updated successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while updating the user status."
         * }
        */
        [HttpPatch("{userId}/status")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Update user status by ID.")]
        public IActionResult UpdateUserStatusById(
            [FromRoute] string userId,
            [FromBody] UpdateUserStatusRequestDto updateUserStatusDto
        )
        {
            try
            {
                // TODO: Implement the logic for updating user status by ID

                const string activity = "User status updated successfully.";
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
         * Reset user password by ID endpoint allows resetting the password of a user by their ID.
         *
         * @param userId The ID of the user to reset password.
         * @param resetPasswordDto The request body containing new password data.
         * @returns {IActionResult} 200 if resetting is successful, 500 if an error occurred.
         * @route PATCH /{userId}/reset
         * @example response - 200 - User password reset successfully
         * {
         *   "statusCode": 200,
         *   "message": "User password reset successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while resetting the user password."
         * }
        */
        [HttpPatch("{userId}/reset")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Reset user password by ID.")]
        public IActionResult ResetUserPasswordById([FromRoute] string userId)
        {
            try
            {
                // TODO: Implement the logic for resetting user password by ID

                const string activity = "User password reset successfully.";
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
