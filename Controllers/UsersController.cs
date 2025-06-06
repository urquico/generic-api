using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Users;
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
    [SwaggerTag("Self User Endpoints")]
    public class UsersController(
        TokenService tokenService,
        UsersService usersService,
        IConfiguration configuration
    ) : ControllerBase
    {
        private readonly ApiResponse _response = new(new HttpContextAccessor());
        private readonly TokenService _tokenService = tokenService;
        private readonly UsersService _usersService = usersService;
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _context = new();

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
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.SELF_UPDATE)]
        public IActionResult PatchUserInfo([FromBody] UpdateUserInfoRequestDto updateUserInfoDto)
        {
            try
            {
                var accessToken = HttpContext.Request.Cookies["accessToken"] ?? "";
                var userId = _tokenService.GetUserFromAccessToken(accessToken).Id;
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                // update user information
                return _usersService.UpdateUser(userId: userId, user: updateUserInfoDto, ip: ip);
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * ChangePassword endpoint allows the authenticated user to change their password.
         *
         * @returns {IActionResult} 201 if password changed successfully, 500 if an error occurred.
         * @route PATCH /me/password
         */
        [HttpPatch("me/password")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = UsersSummary.SELF_CHANGE_PASSWORD)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequestDto changePasswordDto
        )
        {
            try
            {
                var accessToken = HttpContext.Request.Cookies["accessToken"] ?? "";
                var loggedUser = _tokenService.GetUserFromAccessToken(accessToken);

                return await _usersService.ChangePassword(
                    userId: loggedUser.Id,
                    password: changePasswordDto.Password,
                    confirmPassword: changePasswordDto.ConfirmPassword
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }
    }
}
