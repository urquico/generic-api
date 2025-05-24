using System;
using System.Collections.Generic;

namespace GenericApi.Models;

public partial class SecurityQuestion
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public int? QuestionStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public virtual ICollection<UserSecurityQuestion> UserSecurityQuestions { get; set; } = new List<UserSecurityQuestion>();
}
