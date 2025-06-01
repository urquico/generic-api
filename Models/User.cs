using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public virtual KeyCategory? Status { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    public virtual ICollection<UserSecurityQuestion> UserSecurityQuestions { get; set; } = [];

    public virtual ICollection<UserSpecialPermission> UserSpecialPermissions { get; set; } = [];
}
