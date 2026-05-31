using System.ComponentModel.DataAnnotations;

namespace Project_6_final.DTOs
{
    public class AddAddressDTO
    {
        [Required]
        public string Street { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string State { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{6}(-\d{4})?$", ErrorMessage = "Invalid postal code format.")]
        public string PostalCode { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;
    }
    public class UpdateAddressDTO
    {
        [Required]
        public Guid AddressId { get; set; }

        [Required]
        public string Street { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string State { get; set; } = null!;

        [Required]
        //[StringLength(6, MinimumLength = 6, ErrorMessage = "Postal code must be exactly 6 characters long.")]
        public string PostalCode { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;
    }
    public class AddressDTO
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
