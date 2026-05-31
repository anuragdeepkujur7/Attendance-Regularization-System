using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Attendance
{
    public Guid AttendanceId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly? AttendanceDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public virtual ICollection<Regularizationrequest> Regularizationrequests { get; set; } = new List<Regularizationrequest>();

    public virtual User User { get; set; } = null!;
}
