namespace Project_6_final.DTOs
{
    public class MemberAttendanceReportDTO
    {
        public Guid MemberId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }

        public Guid ReportId { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public int? TotalPresent { get; set; }

        public int? TotalAbsent { get; set; }

        public int? TotalLeaves { get; set; }

        public int? TotalRegularized { get; set; }

        public DateTime GeneratedAt { get; set; }
    }
}
