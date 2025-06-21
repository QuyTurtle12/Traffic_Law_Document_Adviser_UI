using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class DocumentTag
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? ParentTagId { get; set; }

    public string? CreatedBy { get; set; }

    public string? LastUpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? LastUpdatedTime { get; set; }

    public DateTime? DeletedTime { get; set; }

    public virtual ICollection<DocumentTagMap> DocumentTagMaps { get; set; } = new List<DocumentTagMap>();

    public virtual ICollection<DocumentTag> InverseParentTag { get; set; } = new List<DocumentTag>();

    public virtual DocumentTag? ParentTag { get; set; }
}
