namespace Project_6_final.DTOs
{
    public class AttendanceDTO  // AttendanceResponseDTO
    {
        public Guid AttendanceId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? AttendanceDate { get; set; }
        public string? Status { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
    public class AttendanceRecordDTO
    {
        public Guid AttendanceId { get; set; }
        public DateOnly? AttendanceDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
    }
    public class AttendanceDetailsDTO
    {
        public Guid MemberId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public ICollection<AttendanceRecordDTO> Attendances { get; set; } = new List<AttendanceRecordDTO>();
    }

    public class AttendanceCreateDto
    {
        public int UserId { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public string Status { get; set; } =  null!; // 'Present' or 'Absent'
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
    public class AttendanceUpdateDTO
    {
        public int AttendanceId { get; set; }
        public int UserId { get; set; }

        public DateOnly? AttendanceDate { get; set; }
        public string Status { get; set; } // 'Present' or 'Absent'
      
    }
    public class AddRegularizationDTO
    {
        public string Reason { get; set; } = string.Empty;
       // public DateTime AttendanceDate { get; set; }
    }
}
