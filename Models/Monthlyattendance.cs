using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Monthlyattendance
{
    public Guid ReportId { get; set; }

    public Guid UserId { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? TotalPresent { get; set; }

    public int? TotalAbsent { get; set; }

    public int? TotalLeave { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
