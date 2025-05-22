using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi.Controllers
{
    [ApiController]
    [Route("api/v1/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly CustomSuccess _response = new();

        // GET: /permissions/all?{query} - Get all permissions
    }
}
