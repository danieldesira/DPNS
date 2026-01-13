namespace DPNS.Entities;

public partial class App
{
    public int Id { get; set; }

    public string AppName { get; set; } = null!;

    public Guid Guid { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual ICollection<PushSubscription> PushSubscriptions { get; set; } = new List<PushSubscription>();
}
