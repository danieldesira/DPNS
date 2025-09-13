using System;
using System.Collections.Generic;

namespace DPNS.DbModels;

public partial class PushSubscription
{
    public int Id { get; set; }

    public string Endpoint { get; set; } = null!;

    public string Auth { get; set; } = null!;

    public string P256dh { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int AppId { get; set; }

    public virtual App App { get; set; } = null!;
}
