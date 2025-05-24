using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class UserSecurityQuestion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SecurityQuestionId { get; set; }

    public string SecurityAnswer { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public virtual SecurityQuestion SecurityQuestion { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
