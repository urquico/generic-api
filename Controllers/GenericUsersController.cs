using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScaffoldTest.Utils;

namespace ScaffoldTest.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class GenericUsersController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        /**
         * ChangePassword endpoint allows the authenticated user to change their password.
         *
         * @returns {IActionResult} 201 if password changed successfully, 500 if an error occurred.
         * @route PATCH /me/password
         * @example response - 201 - Password changed successfully
         * {
         *   "statusCode": 201,
         *   "message": "Password has been changed.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred while changing the password."
         * }
         */
        [HttpPatch("/me/password")]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult ChangePassword()
        {
            try
            {
                // TODO: Implement the logic change password

                const string activity = "Password has been changed.";
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

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
    }
}
