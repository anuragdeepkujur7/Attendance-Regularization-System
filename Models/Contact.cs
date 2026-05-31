using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Contact
{
    public Guid ContactId { get; set; }

    public Guid UserId { get; set; }

    public string Phone { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
