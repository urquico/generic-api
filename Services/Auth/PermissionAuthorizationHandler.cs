using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GenericApi.Services.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement
        )
        {
            var endpoint = context.Resource as Microsoft.AspNetCore.Http.Endpoint;
            var permissionAttribute =
                endpoint?.Metadata.GetMetadata<PermissionAuthorizeAttribute>();
            var requiredPermission = permissionAttribute?.Permission;

            if (
                !string.IsNullOrEmpty(requiredPermission)
                && context.User.HasClaim("permission", requiredPermission)
            )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
