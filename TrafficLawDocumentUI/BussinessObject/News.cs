using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class News
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime PublishedDate { get; set; }

    public string? Author { get; set; }

    public string? ImageUrl { get; set; }

    public string? EmbeddedUrl { get; set; }

    public Guid? UserId { get; set; }

    public string? CreatedBy { get; set; }

    public string? LastUpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? LastUpdatedTime { get; set; }

    public DateTime? DeletedTime { get; set; }

    public Guid? EmbeddedNewsId { get; set; }

    public virtual User? User { get; set; }
}
