using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Utils;
using GenericApi.Utils.Users;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        /**
         * Signup endpoint allows a new user to register.
         *
         * @param signupRequestDto The request body containing user signup information.
         * @returns {IActionResult} 200 if signup is successful, 500 if an error occurred.
         * @route POST /signup
         * @example response - 200 - User signup successful
         * {
         *   "statusCode": 200,
         *   "message": "User signup has been successfully.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred during signup."
         * }
        */
        [HttpPost("/signup")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User signup")]
        public IActionResult Signup([FromBody] SignupRequestDto signupRequestDto)
        {
            try
            {
                // TODO: Implement the logic for user signup

                const string activity = "User signup has been successfully.";
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
