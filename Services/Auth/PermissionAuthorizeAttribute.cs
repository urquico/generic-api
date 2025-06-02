using System;
using System.IO;
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
            // Save the permission to a file for tracking
            try
            {
                var outputPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Permissions",
                    "used_permissions.txt"
                );
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                // Append the permission if not already present
                var existing = File.Exists(outputPath)
                    ? [.. File.ReadAllLines(outputPath)]
                    : new HashSet<string>();
                if (!existing.Contains(permission))
                {
                    File.AppendAllText(outputPath, permission + Environment.NewLine);
                }
            }
            catch
            {
                /* Ignore file errors to not break authorization */
            }
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
