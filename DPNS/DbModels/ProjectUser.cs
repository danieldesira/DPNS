using System;
using System.Collections.Generic;

namespace DPNS.DbModels;

public partial class ProjectUser
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
