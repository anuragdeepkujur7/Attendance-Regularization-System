using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class LoginUserDTO
    {
        
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
