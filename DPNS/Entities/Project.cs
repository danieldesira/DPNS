using System;
using System.Collections.Generic;

namespace DPNS.Entities;

public partial class Project
{
    public int Id { get; set; }

    public string ProjectName { get; set; } = null!;

    public Guid Guid { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
