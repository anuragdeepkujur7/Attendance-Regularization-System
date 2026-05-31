using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class UserAttendanceDTO
    {
        public Guid EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? AttendanceDate { get; set; }
        public string? Status { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
    public class UserAttendanceReportDTO
    {
        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }
    }
   


}
