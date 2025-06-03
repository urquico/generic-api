using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.RolesManagement;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
using GenericApi.Utils.Roles;
using GenericApi.Utils.SwaggerSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/roles")]
    [SwaggerTag("Roles Management")]
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
        */
        [HttpPost]
        [PermissionAuthorize("Admin.CanCreateRole")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = RoleSummary.CREATE)]
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
                            permissionId => new RoleModulePermission
                            {
                                PermissionId = permissionId,
                                CreatedBy = user?.Id,
                                CreatedAt = DateTime.UtcNow,
                            }
                        ),
                    ],
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user?.Id, // Use the user ID from the token
                };

                _context.Roles.Add(newRole);
                _context.SaveChanges();

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: StatusCodes.Status201Created,
                    activity: string.Format(CreateRoleMessages.ACTIVITY_LOG, newRole.RoleName),
                    ip: ip,
                    message: CreateRoleMessages.SUCCESS,
                    data: new CreateRoleResponseDto
                    {
                        RoleName = newRole.RoleName,
                        RoleId = newRole.Id,
                        Permissions =
                        [
                            .. newRole.RoleModulePermissions.Select(rmp => new CreatedPermissionDto
                            {
                                PermissionId = rmp.PermissionId,
                                PermissionName =
                                    _context
                                        .ModulePermissions.FirstOrDefault(mp =>
                                            mp.Id == rmp.PermissionId
                                        )
                                        ?.PermissionName ?? "Unknown",
                            }),
                        ],
                    }
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * @param {string} roleId - The ID of the role to update.
         * @returns {IActionResult} 200 if role updated successfully, 500 if an error occurred.
         * @route PATCH /:roleId
        */
        [HttpPatch("{roleId}")]
        [PermissionAuthorize("Admin.CanUpdateRole")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = RoleSummary.UPDATE)]
        public IActionResult UpdateRole(string roleId, [FromBody] UpdateRoleRequestDto dto)
        {
            try
            {
                var role = _context
                    .Roles.Include(r => r.RoleModulePermissions)
                    .FirstOrDefault(r => r.Id.ToString() == roleId);

                if (role == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(UpdateRoleMessages.ROLE_NOT_FOUND)
                    );
                }

                // Update role info
                role.RoleName = dto.RoleName;
                role.RoleStatus = dto.RoleStatus;
                role.UpdatedAt = DateTime.UtcNow;
                role.UpdatedBy = _tokenService
                    .GetUserFromAccessToken(Request.Cookies["accessToken"] ?? string.Empty)
                    ?.Id;

                // Update permissions
                _context.RoleModulePermissions.RemoveRange(role.RoleModulePermissions);
                role.RoleModulePermissions =
                [
                    .. dto.Permissions.Select(pid => new RoleModulePermission
                    {
                        PermissionId = pid,
                        RoleId = role.Id,
                        CreatedAt = DateTime.UtcNow,
                    }),
                ];

                _context.SaveChanges();

                var updatedPermissions = role
                    .RoleModulePermissions.Select(rmp => new CreatedPermissionDto
                    {
                        PermissionId = rmp.PermissionId,
                        PermissionName =
                            _context
                                .ModulePermissions.FirstOrDefault(mp => mp.Id == rmp.PermissionId)
                                ?.PermissionName ?? "Unknown",
                    })
                    .ToList();

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: string.Format(UpdateRoleMessages.ACTIVITY_LOG, role.RoleName),
                    ip: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP",
                    message: UpdateRoleMessages.SUCCESS,
                    data: new UpdateRoleResponseDto
                    {
                        RoleName = role.RoleName,
                        RoleId = role.Id,
                        Permissions = updatedPermissions,
                    }
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
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
