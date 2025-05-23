using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        /**
         * GetAllPermissions endpoint retrieves all permissions.
         *
         * @returns {IActionResult} 200 if permissions retrieved successfully, 500 if an error occurred.
         * @route GET /all
         * @example response - 200 - Permissions retrieved successfully
         * {
         *   "statusCode": 200,
         *   "message": "Permissions have been retrieved successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while retrieving permissions."
         * }
        */

        [HttpGet("all")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Retrieve all permissions.")]
        public IActionResult GetAllPermissions()
        {
            try
            {
                // TODO: Implement the logic to retrieve all permissions

                const string activity = "Permissions have been retrieved successfully.";
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
