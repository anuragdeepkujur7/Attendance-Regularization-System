using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class RegularizationDTO //user
    {

        [Required]
        public DateTime Date { get; set; } // Attendance date the user wants to regularize

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending"; 
    }
    public class AdminRegularizationDTO
    {
        
        [Required(ErrorMessage = "StatusType is required.")]
        [EnumDataType(typeof(StatusTypeEnum), ErrorMessage = "StatusType must be one of the following: Pending, Approved, Denied.")]
        public string Status { get; set; } = null!;

        public enum StatusTypeEnum
        {
            Pending,
            Approved,
            Denied
        }
    }
   public class RegularizationRequestDTO
    {

        public string Reason { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }

    }
    public class RegularizationResultDTO
    {
        public bool IsSuccessful { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Status { get; set; }
    }

}
