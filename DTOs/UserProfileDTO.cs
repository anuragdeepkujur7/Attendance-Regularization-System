namespace Project_6_final.DTOs
{
    public class UserProfileDTO
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        //public string Email { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;

        public string? MaritalStatus { get; set; }
        public List<string> Roles { get; set; }
        public ContactDTO? Contacts { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
    }
}
