namespace DPNS.Entities;

public partial class AppUser
{
    public int Id { get; set; }

    public int AppId { get; set; }

    public int UserId { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual App App { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
