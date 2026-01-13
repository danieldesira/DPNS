namespace DPNS.Entities;

public partial class ProjectUser
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
