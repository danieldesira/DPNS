using System;
using System.Collections.Generic;

namespace DPNS.DbModels;

public partial class UserVerificationToken
{
    public string VerificationCode { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public int Id { get; set; }

    public virtual User User { get; set; } = null!;
}
