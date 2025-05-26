using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GenericApi.Services.Auth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string Permission { get; }

        public PermissionAuthorizeAttribute(string permission)
        {
            Permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = user.Claims.Any(c =>
                c.Type == "permission" && c.Value == Permission
            );
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
