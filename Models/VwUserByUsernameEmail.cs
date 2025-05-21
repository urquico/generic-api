using System;
using System.Collections.Generic;

namespace ScaffoldTest.Models;

public partial class VwUserByUsernameEmail
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public DateOnly? Birthday { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }
}
