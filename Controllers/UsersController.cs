using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Users;
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
    [SwaggerTag("Self User Endpoints")]
    public class UsersController(TokenService tokenService, UsersService usersService)
        : ControllerBase
    {
        private readonly CustomSuccess _response = new();
        private readonly TokenService _tokenService = tokenService;
        private readonly UsersService _usersService = usersService;

        /**
         * GetUserInfo endpoint retrieves the authenticated user's information.
         *
         * @returns {IActionResult} 200 if user information retrieved successfully, 500 if an error occurred.
         * @route GET /me
        */
        [HttpGet("me")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.SELF_INFO)]
        public IActionResult GetUserInfo()
        {
            try
            {
                var accessToken = HttpContext.Request.Cookies["accessToken"] ?? "";
                var userId = _tokenService.GetUserFromAccessToken(accessToken).Id;

                return _usersService.GetUserById(userId);
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * PatchUserInfo endpoint allows the authenticated user to update their information.
         *
         * @returns {IActionResult} 204 if user information updated successfully, 500 if an error occurred.
         * @route PATCH /me
        */
        [HttpPatch("me")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = UsersSummary.SELF_UPDATE)]
        public IActionResult PatchUserInfo([FromBody] UpdateUserInfoRequestDto updateUserInfoDto)
        {
            try
            {
                // TODO: Implement the logic change password

                const string activity = "User information has been updated successfully.";
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
        [HttpPatch("me/password")]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = UsersSummary.SELF_CHANGE_PASSWORD)]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequestDto changePasswordDto)
        {
            try
            {
                // Validate the request
                if (changePasswordDto.OldPassword == "")
                {
                    return _response.Error(
                        statusCode: 400,
                        e: new Exception(UserMessages.PASSWORD_MUST_NOT_BE_EMPTY)
                    );
                }

                if (changePasswordDto.NewPassword == "")
                {
                    return _response.Error(
                        statusCode: 400,
                        e: new Exception(UserMessages.PASSWORD_MUST_NOT_BE_EMPTY)
                    );
                }

                // check if the new password is not the same as the old password
                if (changePasswordDto.OldPassword != changePasswordDto.NewPassword)
                {
                    return _response.Error(
                        statusCode: 400,
                        e: new Exception(UserMessages.PASSWORD_MISMATCH)
                    );
                }

                // TODO: Implement the logic change password

                const string activity = UserMessages.PASSWORD_CHANGED_SUCCESS;
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
    }
}
