using GenericApi.Dtos.Auth;
using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Services.Users;
using GenericApi.Utils;
using GenericApi.Utils.Auth;
using GenericApi.Utils.SwaggerSummary;
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
    public class AuthController(
        TokenService tokenService,
        IConfiguration configuration,
        UsersService usersService
    ) : ControllerBase
    {
        private readonly CustomSuccess _response = new();
        private readonly TokenService _tokenService = tokenService;
        private readonly AppDbContext _context = new();
        private readonly IConfiguration _configuration = configuration;
        private readonly UsersService _usersService = usersService;

        /**
         * Signup endpoint allows a new user to register.
         *
         * @param signupRequestDto The request body containing user signup information.
         * @returns {IActionResult} 200 if signup is successful, 500 if an error occurred.
         * @route POST /signup
        */
        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = AuthSummary.SIGNUP)]
        public IActionResult Signup([FromBody] SignupRequestDto signupRequestDto)
        {
            try
            {
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _usersService.CreateUser(
                    createUser: signupRequestDto,
                    userId: null, // No user ID for signup
                    ip: ip
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Login endpoint allows a user to authenticate and receive an access token.
         *
         * @param loginRequestDto The request body containing user login credentials.
         * @returns {IActionResult} 200 if login is successful, 400 if email not found or password is invalid, 401 if unauthorized, 500 if an error occurred.
         * @route POST /login
        */
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = AuthSummary.LOGIN)]
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                // Check if the email is existing
                var user = _context.Users.FirstOrDefault(u => u.Email == loginRequestDto.Email);
                if (user == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(LoginMessage.EMAIL_NOT_FOUND),
                        saveLog: true
                    );
                }

                // Check if the user password is correct
                var isValid = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password);
                if (!isValid)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status401Unauthorized,
                        e: new Exception(LoginMessage.INVALID_PASSWORD),
                        saveLog: true
                    );
                }

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

                var (accessToken, permissions) = _tokenService.GenerateAccessToken(userDto);
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id, agentName, ip);

                // Set tokens as HttpOnly cookies
                SetRefreshAuthCookies(refreshToken);
                SetAccessAuthCookies(accessToken);

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: string.Format(LoginMessage.LOGIN_ACTIVITY_LOG, loginRequestDto.Email),
                    ip: ip,
                    message: LoginMessage.SUCCESS_LOGIN,
                    data: new { permissions }
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        // POST: refresh
        /**
         * Refresh endpoint allows an authenticated user to refresh their access token using a valid refresh token.
         *
         * @returns {IActionResult} 200 if refresh is successful, 401 if the refresh token is invalid, 500 if an error occurred.
         * @route POST /refresh
        */
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = AuthSummary.REFRESH)]
        public IActionResult Refresh()
        {
            try
            {
                // Get the refresh token from the cookies
                if (!Request.Cookies.TryGetValue("refreshToken", out string? refreshToken))
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status401Unauthorized,
                        e: new Exception(RefreshMessages.MISSING_TOKEN),
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
                        statusCode: StatusCodes.Status401Unauthorized,
                        e: new Exception(RefreshMessages.INVALID_TOKEN),
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
                        statusCode: StatusCodes.Status401Unauthorized,
                        e: new Exception(RefreshMessages.REVOKED_TOKEN),
                        saveLog: true
                    );
                }

                // Generate a new access token
                var user = _context.Users.Find(token.UserId);
                if (user == null)
                {
                    return _response.Error(
                        statusCode: StatusCodes.Status404NotFound,
                        e: new Exception(RefreshMessages.USER_NOT_FOUND),
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
                var (accessToken, permissions) = _tokenService.GenerateAccessToken(userDto);

                // Set the new access token as an HttpOnly cookie
                SetAccessAuthCookies(accessToken);

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    message: RefreshMessages.SUCCESS,
                    data: new { permissions }
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }

        /**
         * Logout endpoint allows an authenticated user to log out of the system.
         *
         * @returns {IActionResult} 200 if logout is successful, 500 if an error occurred.
         * @route POST /logout
        */
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = AuthSummary.LOGOUT)]
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
                            statusCode: StatusCodes.Status401Unauthorized,
                            e: new Exception(LogoutMessages.MISSING_TOKEN),
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

                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    activity: LogoutMessages.ACTIVITY_LOG,
                    ip: ip,
                    message: LogoutMessages.SUCCESS,
                    data: null
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
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
                    Secure = false, // TODO: Set to true in production (requires HTTPS)
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
                    Secure = false, // TODO: Set to true in production (requires HTTPS)
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(double.Parse(expiration)),
                }
            );
        }
    }
}
