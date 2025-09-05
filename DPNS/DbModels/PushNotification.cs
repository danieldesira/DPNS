using System;
using System.Collections.Generic;

namespace DPNS.DbModels;

public partial class PushNotification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
