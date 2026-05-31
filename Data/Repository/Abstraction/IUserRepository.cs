using Project_6_final.Models;
using Project_6_final.DTOs;

namespace Project_6_final.Data.Repository.Abstraction
{
    public interface IUserRepository
    {
       
        Task<bool> RegisterUserAsync(User member);
        Task<List<User>> GetAllUserDetails();  //Done
        Task<bool> CheckUserInDb(string email);
     
 
        Task<User?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAsync2(User user);

        Task<User> GetUserByEmailAsync(string email);
        
        Task<bool> UpdateUserAsync(Guid id, User member);
        
        
        Task<List<User>> GetUserDataAsync(Guid? id);
        Task<bool> UpdatePasswordAsync(User user);
        
        Task InvalidateTokenAsync(string token, Guid userId, DateTime expiration);
        Task<bool> IsTokenValidAsync(string token);
        Task<bool> InvalidateToken(Guid id);
        //CONTACT
        Task<bool> AddContact(Contact contact);
        Task<bool> UpdateContact(Contact contact);

        Task<bool> AddAddress(Address address);
        Task<bool> UpdateAddress(Address address);
        Task<User> GetMemberById(Guid id);
        Task<UserProfileDTO?> GetUserProfile(Guid userId);

    }
}
