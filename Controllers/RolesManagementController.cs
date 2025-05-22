using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesManagementController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        // GET: /roles/all?{query} - Get all roles
        // GET: /roles/:roleId - Get a specific role
        // POST: /roles - Create a new role
        // PATCH: /roles/:roleId - Update a specific role
        // DELETE: /roles/:roleId - Soft Delete a specific role
        // DELETE: /roles/:roleId/force - Force Delete a specific role
        // PATCH: /roles/:roleId/restore - Restore a specific role
    }
}
