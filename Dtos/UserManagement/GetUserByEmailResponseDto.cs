using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GenericApi.Dtos.UserManagement
{
    public class GetUserByEmailResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("middle_name")]
        public string MiddleName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("updated_by")]
        public int? UpdatedBy { get; set; }

        [JsonPropertyName("roles")]
        public List<Role> Roles { get; set; } = [];

        [JsonPropertyName("permissions")]
        public List<Permission> Permissions { get; set; } = [];
    }

    public class Role
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [JsonPropertyName("role_status")]
        public bool RoleStatus { get; set; }
    }

    public class Permission
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("permission_name")]
        public string PermissionName { get; set; } = string.Empty;

        [JsonPropertyName("permission_status")]
        public int PermissionStatus { get; set; }
    }
}
