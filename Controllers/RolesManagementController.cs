using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesManagementController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        // GET: /roles/all?{query} - Get all roles
        /**
         * @query {string} query - Optional query parameter for filtering roles.
         * @returns {IActionResult} 200 if roles retrieved successfully, 500 if an error occurred.
         * @route GET /all
         * @example response - 200 - Roles retrieved successfully
         * {
         *   "statusCode": 200,
         *   "message": "Roles have been retrieved successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while retrieving roles."
         * }
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
        // POST: /roles - Create a new role
        // PATCH: /roles/:roleId - Update a specific role
        // DELETE: /roles/:roleId - Soft Delete a specific role
        // DELETE: /roles/:roleId/force - Force Delete a specific role
        // PATCH: /roles/:roleId/restore - Restore a specific role
    }
}
