using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.Auth;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
using GenericApi.Utils.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UAParser;

namespace GenericApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/auth")]
    [SwaggerTag("Authentication and Authorization")]
    public class AuthController(TokenService tokenService, IConfiguration configuration)
        : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        private readonly TokenService _tokenService = tokenService;

        private readonly AppDbContext _context = new();
        private readonly IConfiguration _configuration = configuration;

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
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User signup")]
        public IActionResult Signup([FromBody] SignupRequestDto signupRequestDto)
        {
            try
            {
                // Check if the email already exists
                var existingUser = _context.Users.FirstOrDefault(u =>
                    u.Email == signupRequestDto.Email
                );
                if (existingUser != null)
                {
                    return _response.Error(
                        statusCode: 400,
                        e: new Exception(AuthMessage.EMAIL_ALREADY_EXISTS),
                        saveLog: true
                    );
                }

                // Check if password and confirm password match
                if (signupRequestDto.Password != signupRequestDto.ConfirmPassword)
                {
                    return _response.Error(
                        statusCode: 400,
                        e: new Exception(AuthMessage.PASSWORD_CONFIRMATION_MISMATCH),
                        saveLog: true
                    );
                }

                var saltRoundsStr = _configuration.GetSection("PasswordHashing")["SaltRounds"];
                int saltRounds = int.TryParse(saltRoundsStr, out var rounds) ? rounds : 12; // fallback to 12
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                    signupRequestDto.Password,
                    workFactor: saltRounds
                );

                // save the new user to the database
                _context.Users.Add(
                    new User
                    {
                        Email = signupRequestDto.Email,
                        Password = hashedPassword,
                        FirstName = signupRequestDto.FirstName,
                        MiddleName = signupRequestDto.MiddleName,
                        LastName = signupRequestDto.LastName,
                        StatusId = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    }
                );

                _context.SaveChanges();

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: 200,
                    activity: AuthMessage.SIGNUP_ACTIVITY_LOG + signupRequestDto.Email,
                    ip: ip,
                    message: AuthMessage.SUCCESS_SIGNUP,
                    data: new SignupResponseDto
                    {
                        Email = signupRequestDto.Email,
                        FirstName = signupRequestDto.FirstName,
                        MiddleName = signupRequestDto.MiddleName,
                        LastName = signupRequestDto.LastName,
                    }
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
        [AllowAnonymous]
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

                var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!isValid)
                {
                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Invalid password."),
                        saveLog: true
                    );
                }

                // Generate Tokens
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                var userAgent = Request.Headers.UserAgent.ToString();
                if (string.IsNullOrEmpty(userAgent))
                {
                    userAgent = "Unknown User Agent";
                }

                var uaParser = Parser.GetDefault();
                ClientInfo clientInfo = uaParser.Parse(userAgent);

                string browser = clientInfo.UA.Family;
                string os = clientInfo.OS.Family;
                string device = clientInfo.Device.Family;

                string agentName = $"{browser} on {os} ({device})";

                var userDto = new UserJwtDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    // Add other properties as needed
                };

                var accessToken = _tokenService.GenerateAccessToken(userDto);
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id, agentName, ip);

                // Set tokens as HttpOnly cookies
                SetRefreshAuthCookies(refreshToken);
                SetAccessAuthCookies(accessToken);

                const string activity = "You have successfully logged in.";

                return _response.Success(
                    statusCode: 200,
                    activity: activity,
                    ip: ip,
                    message: activity,
                    data: new { token = accessToken }
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: 500, e: ex);
            }
        }

        // POST: refresh
        /**
         * Refresh endpoint allows an authenticated user to refresh their access token using a valid refresh token.
         *
         * @returns {IActionResult} 200 if refresh is successful, 401 if the refresh token is invalid, 500 if an error occurred.
         * @route POST /refresh
         * @example response - 200 - Token refreshed successfully
         * {
         *   "statusCode": 200,
         *   "message": "Access token has been refreshed successfully.",
         *   "data": null
         * }
         * @example response - 401 - Invalid refresh token
         * {
         *   "statusCode": 401,
         *   "error": "Invalid refresh token."
         * }
         * @example response - 500 - Error
         * {
         *   "statusCode": 500,
         *   "error": "An error occurred during token refresh."
         * }
        */
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "Refresh access token")]
        public IActionResult Refresh()
        {
            try
            {
                // Get the refresh token from the cookies
                if (!Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
                {
                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Refresh token is missing."),
                        saveLog: true
                    );
                }

                // Validate the refresh token
                var token = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                if (token == null || token.ExpiresAt < DateTime.UtcNow)
                {
                    // remove both tokens
                    Response.Cookies.Delete("refreshToken");
                    Response.Cookies.Delete("accessToken");

                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Invalid or expired refresh token."),
                        saveLog: true
                    );
                }

                // Check if token has revoked_at
                if (token.RevokedAt.HasValue)
                {
                    // remove both tokens from cookies
                    Response.Cookies.Delete("refreshToken");
                    Response.Cookies.Delete("accessToken");

                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("Refresh token has been revoked."),
                        saveLog: true
                    );
                }

                // Generate a new access token
                var user = _context.Users.Find(token.UserId);
                if (user == null)
                {
                    return _response.Error(
                        statusCode: 401,
                        e: new Exception("User not found."),
                        saveLog: true
                    );
                }

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                var userDto = new UserJwtDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    // Add other properties as needed
                };
                var accessToken = _tokenService.GenerateAccessToken(userDto);

                // Set the new access token as an HttpOnly cookie
                SetAccessAuthCookies(accessToken);

                const string activity = "Access token has been refreshed successfully.";

                return _response.Success(
                    statusCode: 200,
                    message: activity,
                    data: new { token = accessToken }
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(object), 500)]
        [SwaggerOperation(Summary = "User logout")]
        public IActionResult Logout()
        {
            try
            {
                // revoke the refresh token and clear the cookies
                if (Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
                {
                    var token = _context.RefreshTokens.FirstOrDefault(rt =>
                        rt.Token == refreshToken
                    );
                    if (token == null)
                    {
                        Response.Cookies.Delete("refreshToken");
                        return _response.Error(
                            statusCode: 401,
                            e: new Exception("Invalid Token."),
                            saveLog: true
                        );
                    }
                    else
                    {
                        // Mark the token as revoked
                        token.RevokedAt = DateTime.UtcNow;
                        token.RevokedBy = token.UserId.ToString();
                        _context.RefreshTokens.Update(token);
                        _context.SaveChanges();
                    }

                    // clear the cookies
                    Response.Cookies.Delete("refreshToken");
                    Response.Cookies.Delete("accessToken");
                }

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

        private void SetRefreshAuthCookies(string refreshToken)
        {
            var expiration = _configuration.GetSection("Jwt")["RefreshTokenExpirationDays"] ?? "7";

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Set to true in production (requires HTTPS)
                    SameSite = SameSiteMode.Strict,
                    // TODO: Change AddMinutes to AddDays for refresh token expiration after testing
                    Expires = DateTimeOffset.UtcNow.AddMinutes(double.Parse(expiration)),
                }
            );
        }

        private void SetAccessAuthCookies(string accessToken)
        {
            var expiration =
                _configuration.GetSection("Jwt")["AccessTokenExpirationMinutes"] ?? "15";

            Response.Cookies.Append(
                "accessToken",
                accessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Set to true in production (requires HTTPS)
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(double.Parse(expiration)),
                }
            );
        }
    }
}
