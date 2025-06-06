using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
using GenericApi.Utils.Permissions;
using GenericApi.Utils.SwaggerSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GenericApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/permissions")]
    [SwaggerTag("Permissions Endpoints")]
    public class PermissionsController : ControllerBase
    {
        private readonly ApiResponse _response = new(new HttpContextAccessor());
        private readonly AppDbContext _context = new();

        /**
         * GetAllPermissions endpoint retrieves all permissions.
         *
         * @returns {IActionResult} 200 if permissions retrieved successfully, 500 if an error occurred.
         * @route GET /all
        */
        [HttpGet("all")]
        [PermissionAuthorize("Admin.CanViewAllPermissions")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = PermissionsSummary.GET_ALL)]
        public IActionResult GetAllPermissions()
        {
            try
            {
                // get all permissions from the database
                var permissions = _context
                    .ModulePermissions.Select(p => new
                    {
                        p.Id,
                        p.PermissionName,
                        PermissionStatus = p.PermissionStatus == 1 ? "Active" : "Inactive",
                        // get module name from the related module
                        ModuleName = p.Module != null ? p.Module.ModuleName : "No Module",
                    })
                    .ToList();

                return _response.Success(
                    statusCode: StatusCodes.Status200OK,
                    message: GetAllPermissionsMessages.SUCCESS,
                    data: permissions
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: StatusCodes.Status500InternalServerError, e: ex);
            }
        }
    }
}
