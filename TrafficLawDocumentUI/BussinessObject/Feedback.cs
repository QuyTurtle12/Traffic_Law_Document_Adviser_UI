using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class Feedback
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ChatHistory { get; set; }

    public string? Aianswer { get; set; }

    public string? Content { get; set; }

    public string? CreatedBy { get; set; }

    public string? LastUpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? LastUpdatedTime { get; set; }

    public DateTime? DeletedTime { get; set; }

    public virtual ChatHistory ChatHistoryNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
