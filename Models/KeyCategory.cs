using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class KeyCategory
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string? CategoryValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
