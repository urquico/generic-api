using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class Module
{
    public int Id { get; set; }

    public string ModuleName { get; set; } = null!;

    public int? GrandParentId { get; set; }

    public int? ParentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public bool? ModuleStatus { get; set; }

    public virtual Module? GrandParent { get; set; }

    public virtual ICollection<Module> InverseGrandParent { get; set; } = new List<Module>();

    public virtual ICollection<Module> InverseParent { get; set; } = new List<Module>();

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();

    public virtual Module? Parent { get; set; }
}
