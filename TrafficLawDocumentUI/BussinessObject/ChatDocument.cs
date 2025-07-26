using System;
using System.Collections.Generic;

namespace BussinessObject;

public partial class ChatDocument
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid LawDocumentId { get; set; }

    public virtual LawDocument LawDocument { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
