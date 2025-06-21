using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class ChatHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public string? CreatedBy { get; set; }

    public string? LastUpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? LastUpdatedTime { get; set; }

    public DateTime? DeletedTime { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User User { get; set; } = null!;
}
