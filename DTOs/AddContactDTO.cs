using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class AddContactDTO
    {
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
    }
    public class UpdateContactDTO
    {
        [Required]
        public Guid ContactId { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
    }
    public class ContactDTO
    {
        public string Phone { get; set; } = null!;
    }
}
