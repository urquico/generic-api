using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedBy { get; set; }

    public virtual User User { get; set; } = null!;
}
