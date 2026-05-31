using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Invalidtoken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
