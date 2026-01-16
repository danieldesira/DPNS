using Microsoft.AspNetCore.Identity;

namespace DPNS.Entities;

public partial class User : IdentityUser<int>
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? HashedPassword { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = [];

    public virtual ICollection<UserVerificationToken> UserVerificationTokens { get; set; } = [];
}
