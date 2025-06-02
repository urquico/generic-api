using GenericApi.Models;
using GenericApi.Services.Auth;
using GenericApi.Utils;
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
        private readonly CustomSuccess _response = new();
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
        [SwaggerOperation(Summary = "Retrieve all permissions.")]
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
                        // get breadcrumbs for the module, combine the grandparent and parent names from the module
                    })
                    .ToList();

                return _response.Success(
                    statusCode: 200,
                    message: "Permissions retrieved successfully.",
                    data: permissions
                );
            }
            catch (Exception ex)
            {
                return _response.Error(statusCode: 500, e: ex);
            }
        }
    }
}
