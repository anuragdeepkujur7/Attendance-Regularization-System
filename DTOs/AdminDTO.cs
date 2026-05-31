using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Project_6_final.DTOs
{
    
    
    //don't us now 
    public class AdminDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        /*[Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must be at least 6 characters, including letters and numbers.")]
        public string Password { get; set; } = null!;*/
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters and at most 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one letter, one number, and be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(RoleEnum2), ErrorMessage = "Invalid role. Valid roles are Admin or User.")]

        public string Role { get; set; }
    }
    public enum RoleEnum2
    {
        Admin,
        User
    }
}
   



