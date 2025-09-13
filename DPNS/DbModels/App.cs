using System;
using System.Collections.Generic;

namespace DPNS.DbModels;

public partial class App
{
    public int Id { get; set; }

    public string AppName { get; set; } = null!;

    public Guid Guid { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string PublicKey { get; set; } = null!;

    public string PrivateKey { get; set; } = null!;

    public string Url { get; set; } = null!;

    public virtual ICollection<PushSubscription> PushSubscriptions { get; set; } = new List<PushSubscription>();
}
