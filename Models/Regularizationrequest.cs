using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Regularizationrequest
{
    public Guid RequestId { get; set; }

    public Guid UserId { get; set; }

    public Guid AttendanceId { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid RequestedBy { get; set; }

    public virtual Attendance Attendance { get; set; } = null!;

    public virtual User RequestedByNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
