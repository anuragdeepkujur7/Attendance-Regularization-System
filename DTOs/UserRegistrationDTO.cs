using Project_6_final.Models;
using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class UserRegistrationDTO
    {
        
        [Required(ErrorMessage = "FirstName is required.")]
        public string? FirstName { get; set; }



        [Required(ErrorMessage = "LastName is required.")]
        public string? LastName { get; set; }



        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters and at most 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one letter, one number, and be at least 8 characters.")]
        public string Password { get; set; }

        
        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(RoleEnum), ErrorMessage = "Role must be either 'Admin' or 'User'.")]
        public string Role { get; set; }
    }


    public enum RoleEnum
    {
        Admin,
        User
    }



    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        //public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
       
    }
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender. Valid options are: Male, Female, Other.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Marital Status is required.")]
        [EnumDataType(typeof(MaritalStatus), ErrorMessage = "Invalid marital status. Valid options are: Single, Married, Divorced.")]
        public string MaritalStatus { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum MaritalStatus
    {
        Single,
        Married,
        Divorced
    }
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and at most 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one letter, one number, and be at least 8 characters.")]
        public string NewPassword { get; set; }=string.Empty;


        [Compare("NewPassword", ErrorMessage = "Password confirmation does not match the new password.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public class PasswordRecoveryDTO
    {
        public string Email { get; set; }
    }
}
