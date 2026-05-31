using Microsoft.AspNetCore.Identity;
using Project_6_final.Service.Abstraction;

namespace Project_6_final.Helper
{
    public class PasswordHasher: IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
