using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class ModulePermission
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;

    public int? PermissionStatus { get; set; }

    public int? ModuleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public virtual Module? Module { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } =
        new List<RolePermission>();
}
