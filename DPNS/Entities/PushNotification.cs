namespace DPNS.Entities;

public partial class PushNotification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string AppUrl { get; set; } = null!;

    public string UserEmail { get; set; } = null!;
}
