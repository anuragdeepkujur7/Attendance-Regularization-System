using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class Education
{
    public Guid EducationId { get; set; }

    public Guid UserId { get; set; }

    public string Institution { get; set; } = null!;

    public string Degree { get; set; } = null!;

    public string? FieldOfStudy { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? Percentage { get; set; }

    public virtual User User { get; set; } = null!;
}
