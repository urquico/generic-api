using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(TokenService tokenService) : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        private readonly TokenService _tokenService = tokenService;

        private readonly AppDbContext _context = new();

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
        [HttpPost("signup")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User signup")]
        public IActionResult Signup([FromBody] SignupRequestDto signupRequestDto)
        {
            try
            {
                // TODO: Implement the logic for user signup

                const string activity = "Your account has been created successfully.";
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
         * Login endpoint allows a user to authenticate and receive an access token.
         *
         * @param loginRequestDto The request body containing user login credentials.
         * @returns {IActionResult} 200 if login is successful, 500 if an error occurred.
         * @route POST /login
         * @example response - 200 - User login successful
         * {
         *   "statusCode": 200,
         *   "message": "You have successfully logged in.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred during login."
         * }
        */
        [HttpPost("login")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                // validate the user credentials
                var email = loginRequestDto.Email;
                var password = loginRequestDto.Password;

                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Invalid email."),
                        saveLog: true
                    );
                }

                // TODO: Implement password hashing and verification
                // check if the password matches the stored password
                if (user.Password != password)
                {
                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Invalid password."),
                        saveLog: true
                    );
                }

                // Generate Tokens
                var accessToken = _tokenService.GenerateAccessToken(email);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Set tokens as HttpOnly cookies
                SetAuthCookies(accessToken, refreshToken);

                const string activity = "You have successfully logged in.";
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
         * Logout endpoint allows an authenticated user to log out of the system.
         *
         * @returns {IActionResult} 200 if logout is successful, 500 if an error occurred.
         * @route POST /logout
         * @example response - 200 - User logout successful
         * {
         *   "statusCode": 200,
         *   "message": "You have successfully logged out.",
         *   "data": null
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred during logout."
         * }
        */
        [HttpPost("logout")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User logout")]
        public IActionResult Logout()
        {
            try
            {
                // TODO: Implement the logic for user logout

                const string activity = "You have successfully logged out.";
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

        private void SetAuthCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append(
                "accessToken",
                accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Set to true in production (requires HTTPS)
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(2),
                }
            );

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Set to true in production (requires HTTPS)
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                }
            );
        }
    }
}
