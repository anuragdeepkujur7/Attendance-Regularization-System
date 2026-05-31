using Project_6_final.Models;
using Project_6_final.DTOs;
namespace Project_6_final.Service.Abstraction
{
    public interface IUserService
    {
        //Task RegisterUserAsync(UserRegistrationDTO dto);
        //Task<User> RegisterUserAsync(UserRegistrationDTO userDto); // hanlde usr registration logic
        Task<bool> RegisterUserAsync(UserRegistrationDTO registerUser);
       
        Task<string> CreateUserAsync(UserRegistrationDTO adminDTO);
        Task<string> LoginUserAsync(LoginUserDTO loginUserDTO);
        Task<bool> LogoutUserAsync();
        Task<List<UserResponseDTO>> GetAllUserData();



        Task<UserResponseDTO> GetUserProfileAsync(Guid? userId);
        //Task<bool> UpdateUserProfileAsync(Guid userId, UserUpdateDTO profileUpdateDto);
        Task<User> UpdateUserProfileAsync(Guid userId, UserUpdateDTO profileUpdateDto);
        //Task<string> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);
        Task<string> PasswordRecoveryAsync(PasswordRecoveryDTO dto);

        /* Task<string> LogoutAsync(string token, DateTime expiration);
         Task<bool> ValidateTokenAsync(string token);*/
        //Task InvalidateTokenAsync(string token, int userId, DateTime expiration);
        Task InvalidateTokenAsync(string token, Guid userId, DateTime expiration);
        Task<bool> IsTokenValidAsync(string token);

        //Task<bool> UpdateRequestStatus(int empid, Guid requestId, bool isApproved); done in regularzationservice

        //CONTACT
        Task<bool> AddContactForUser(AddContactDTO addContactDTO);
        Task<bool> UpdateContactForUser(UpdateContactDTO updateContactDTO);

        //ADDRESS
        Task<bool> AddAddressForUser(AddAddressDTO addAddressDTO);
        Task<bool> UpdateAddressForUser(UpdateAddressDTO updateAddressDTO);
        Task<User> GetMemberById(Guid id);
        Task<UserProfileDTO?> GetUserProfile();
    }
}
