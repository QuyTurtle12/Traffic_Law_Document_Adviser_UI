using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class LawDocument
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? DocumentCode { get; set; }

    public Guid? CategoryId { get; set; }

    public string? FilePath { get; set; }

    public string? LinkPath { get; set; }

    public bool ExpertVerification { get; set; }

    public string? CreatedBy { get; set; }

    public string? LastUpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? LastUpdatedTime { get; set; }

    public DateTime? DeletedTime { get; set; }

    public virtual DocumentCategory? Category { get; set; }

    public virtual ICollection<ChatDocument> ChatDocuments { get; set; } = new List<ChatDocument>();

    public virtual ICollection<DocumentTagMap> DocumentTagMaps { get; set; } = new List<DocumentTagMap>();
}
