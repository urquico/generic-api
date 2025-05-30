using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.RolesManagement;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesManagementController(TokenService tokenService) : ControllerBase
    {
        private readonly CustomSuccess _response = new();
        private readonly AppDbContext _context = new();

        private readonly TokenService _tokenService = tokenService;

        /**
         * @query {string} query - Optional query parameter for filtering roles.
         * @returns {IActionResult} 200 if roles retrieved successfully, 500 if an error occurred.
         * @route GET /all
        */
        [HttpGet("all")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult GetAllRoles([FromQuery] string query)
        {
            try
            {
                // TODO: Implement the logic to retrieve all roles

                const string activity = "Roles have been retrieved successfully.";
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

        // GET: /roles/:roleId - Get a specific role
        /**
         * @param {string} roleId - The ID of the role to retrieve.
         * @returns {IActionResult} 200 if role retrieved successfully, 500 if an error occurred.
         * @route GET /:roleId
         * @example response - 200 - Role retrieved successfully
         * {
         *   "statusCode": 200,
         *   "message": "Role has been retrieved successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while retrieving the role."
         * }
        */
        [HttpGet("{roleId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult GetRole(string roleId)
        {
            try
            {
                // TODO: Implement the logic to retrieve a specific role

                const string activity = "Role has been retrieved successfully.";
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
         * @returns {IActionResult} 201 if role created successfully, 500 if an error occurred.
         * @route POST /
         * @example response - 201 - Role created successfully
         * {
         *   "statusCode": 201,
         *   "message": "Role has been created successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while creating the role."
         * }
        */
        [HttpPost]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult CreateRole([FromBody] CreateRoleRequestDto createRoleRequestDto)
        {
            try
            {
                var accessToken = Request.Cookies["accessToken"];
                var user = _tokenService.GetUserFromAccessToken(accessToken ?? string.Empty);

                // Insert the new role into the database
                var newRole = new Role
                {
                    RoleName = createRoleRequestDto.RoleName,
                    RoleStatus = true, // Active by default
                    RoleModulePermissions =
                    [
                        .. createRoleRequestDto.Permissions.Select(
                            permissionId => new RoleModulePermission { PermissionId = permissionId }
                        ),
                    ],
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user?.Id, // Use the user ID from the token
                };

                _context.Roles.Add(newRole);
                _context.SaveChanges();

                const string activity = "Role has been created successfully.";
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 201,
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
         * @param {string} roleId - The ID of the role to update.
         * @returns {IActionResult} 200 if role updated successfully, 500 if an error occurred.
         * @route PATCH /:roleId
         * @example response - 200 - Role updated successfully
         * {
         *   "statusCode": 200,
         *   "message": "Role has been updated successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while updating the role."
         * }
        */
        [HttpPatch("{roleId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult UpdateRole(string roleId)
        {
            try
            {
                // TODO: Implement the logic to update a specific role

                const string activity = "Role has been updated successfully.";
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

        // DELETE: /roles/:roleId - Soft Delete a specific role
        /**
         * @param {string} roleId - The ID of the role to delete.
         * @returns {IActionResult} 200 if role deleted successfully, 500 if an error occurred.
         * @route DELETE /:roleId
         * @example response - 200 - Role deleted successfully
         * {
         *   "statusCode": 200,
         *   "message": "Role has been deleted successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while deleting the role."
         * }
        */
        [HttpDelete("{roleId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult DeleteRole(string roleId)
        {
            try
            {
                // TODO: Implement the logic to soft delete a specific role

                const string activity = "Role has been deleted successfully.";
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
         * @param {string} roleId - The ID of the role to force delete.
         * @returns {IActionResult} 200 if role force deleted successfully, 500 if an error occurred.
         * @route DELETE /:roleId/force
         * @example response - 200 - Role force deleted successfully
         * {
         *   "statusCode": 200,
         *   "message": "Role has been force deleted successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while force deleting the role."
         * }
        */
        [HttpDelete("{roleId}/force")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult ForceDeleteRole(string roleId)
        {
            try
            {
                // TODO: Implement the logic to force delete a specific role
                const string activity = "Role has been force deleted successfully.";
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
         * @param {string} roleId - The ID of the role to restore.
         * @returns {IActionResult} 200 if role restored successfully, 500 if an error occurred.
         * @route PATCH /:roleId/restore
         * @example response - 200 - Role restored successfully
         * {
         *   "statusCode": 200,
         *   "message": "Role has been restored successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while restoring the role."
         * }
        */
        [HttpPatch("{roleId}/restore")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult RestoreRole(string roleId)
        {
            try
            {
                // TODO: Implement the logic to restore a specific role

                const string activity = "Role has been restored successfully.";
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
