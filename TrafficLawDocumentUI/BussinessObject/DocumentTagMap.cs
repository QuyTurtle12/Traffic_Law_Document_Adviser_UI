using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class DocumentTagMap
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public Guid DocumentTagId { get; set; }

    public virtual LawDocument Document { get; set; } = null!;

    public virtual DocumentTag DocumentTag { get; set; } = null!;
}
