using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project_6_final.Data.Repository;
using Project_6_final.Data.Repository.Abstraction;
using Project_6_final.DTOs;
using Project_6_final.Models;
using Project_6_final.Service.Abstraction;
using Project_6_final.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Project_6_final.Service.Implementation.UserService;

namespace Project_6_final.Service.Implementation
{
    public class UserService : IUserService

    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // For retrieving secret key from appsettings.json
        private readonly TokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, TokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> CreateUserAsync(UserRegistrationDTO userDTO)  // for user
        {
            // Check if the user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(userDTO.Email);
            if (existingUser != null)
                return "User with this email already exists.";

            // Create a new user object
            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                //PasswordHash=userDTO.Password,
                Roles = "User"
            };


            // Hash the password using BCrypt.Net


            // Save the new user to the database
            await _userRepository.RegisterUserAsync(user);

            return $"User '{userDTO.FirstName}' created successfully."; //with ID: {userDTO.UserId}.";
        }

        public async Task<bool> RegisterUserAsync(UserRegistrationDTO registerUser)   // for admin to create user in use
        {
            /* try
             {

                 var check = await _userRepository.CheckUserInDb(registerUser.Email);

                 if (check)
                 {
                     return false;
                     //throw new Exception("User already registered");
                 }



                 //Adding Member details
                 Guid memberId = Guid.NewGuid();
                 User member = new User();
                 member.UserId = memberId;
                 member.FirstName = registerUser.FirstName;
                 member.LastName = registerUser.LastName;
                 member.Email = registerUser.Email;
                 member.PasswordHash = BCrypt.Net.BCrypt.HashPassword(null, registerUser.Password);
                 member.Roles = "user";


                 bool result = await _userRepository.RegisterUserAsync(member);
                 if (!result)
                 {
                     throw new Exception("User not registered..");
                 }
                 return true;
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.InnerException);
                 return false;
             }*/

            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(registerUser.Email);
            if (existingUser != null)
            {
                return false; // Email already exists
            }

            // Hash the password
            var hashedPassword = _passwordHasher.HashPassword(registerUser.Password);

            // Create the User entity
            var user = new User
            {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email,
                PasswordHash = hashedPassword,
                Roles = registerUser.Role,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            // Save user to the database
            return await _userRepository.RegisterUserAsync(user);
        }
     

      
        public async Task<string> LoginUserAsync(LoginUserDTO loginUserDTO)  
        {
            var user = await _userRepository.GetUserByEmailAsync(loginUserDTO.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid User");
            }
            else
            {

                var result = BCrypt.Net.BCrypt.Verify(loginUserDTO.Password,user.PasswordHash);
                if (!result)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

            }

            string token = await _jwtTokenGenerator.GenerateToken(user.UserId, user.Roles, user.Email);
            return token ;


            /*if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid email or password.");
            }*/

            // Generate JWT Token
            // return GenerateJwtToken(user);
            /*var claims = new[]
             {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Roles)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
                );
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;*/



        }
        public async Task<List<UserResponseDTO>> GetAllUserData()
        {
            var members = await _userRepository.GetAllUserDetails();

            var userDetailsDTOs = new List<UserResponseDTO>();

            foreach (var member in members)
            {
                UserResponseDTO userResponseDTO = new UserResponseDTO()
                {
                    //UserId = member.MemberId,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    DateOfBirth = member.DateOfBirth,
                    Gender = member.Gender,

                    // add later after scaffold


                };

                userDetailsDTOs.Add(userResponseDTO);
            }
            
            return userDetailsDTOs;
        }

        public async Task<User> UpdateUserProfileAsync(Guid userId, UserUpdateDTO profileUpdateDto)
        {
            try
            {
                Guid memId = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //Updating Member/User Data
                User user = new User();
                user.UserId = memId;
                user.FirstName = profileUpdateDto.FirstName ?? user.FirstName;
                user.LastName = profileUpdateDto.LastName ?? user.LastName;
               
                user.DateOfBirth = profileUpdateDto.DateOfBirth ?? user.DateOfBirth;
                user.Gender = profileUpdateDto.Gender ?? user.Gender;

                bool response = await _userRepository.UpdateUserAsync(memId, user);

                if (!response)
                {
                    throw new Exception("Update failed!");
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return null;

            }
        }

        public async Task<UserResponseDTO> GetUserProfileAsync(Guid? userId)
        {
            var user = await _userRepository.GetUserDataAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var userDetails = user.FirstOrDefault();

            if (userDetails == null)
            {
                throw new KeyNotFoundException("User details not found.");
            }

            return new UserResponseDTO
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                DateOfBirth = userDetails.DateOfBirth,
                Gender = userDetails.Gender,
                Street = userDetails.Addresses?.FirstOrDefault()?.Street,
                PostalCode = userDetails.Addresses?.FirstOrDefault()?.PostalCode,
                City = userDetails.Addresses?.FirstOrDefault()?.City,
                State = userDetails.Addresses?.FirstOrDefault()?.State,
                Country = userDetails.Addresses?.FirstOrDefault()?.Country,
                Phone = userDetails.Contacts?.FirstOrDefault()?.Phone,
                //Institution = userDetails.Educations?.FirstOrDefault()?.Institution,
                //Board = userDetails.Educations?.FirstOrDefault()?.Board,
                //Degree = userDetails.Educations?.FirstOrDefault()?.Degree,
               // FieldOfStudy = userDetails.Educations?.FirstOrDefault()?.FieldOfStudy,
                //Grade = userDetails.Educations?.FirstOrDefault()?.Grade
            };
        }

        public async Task<User> GetMemberById(Guid id)
        {
            User member = await _userRepository.GetMemberById(id);
            return member;
        }
        public async Task<UserProfileDTO?> GetUserProfile()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new FormatException("User ID claim is not in the correct format.");
            }

            
            return await _userRepository.GetUserProfile(userId);
            /*User memberDetails = await GetMemberById(userId);

            UserProfileDTO userDetailsDTO = new UserProfileDTO()
            {
                UserId = memberDetails.UserId,
                FirstName = memberDetails.FirstName,
                LastName = memberDetails.LastName,
                DateOfBirth = memberDetails.DateOfBirth,
                Gender = memberDetails.Gender,
                //MaritalStatus = memberDetails.MaritalStatus,
                //CreatedOn = memberDetails.CreatedAt,
                //Roles = memberDetails.Roles.Select(mr => mr.Roles.RoleName).ToList(),

                Contacts = memberDetails.Contacts != null ? new ContactDTO
                {
                    Phone = memberDetails.Contacts.First().Phone,
                } : null,
                *//*.Select(contact => new ContactDTO
                {
                    Phone = contact.Phone,
                    IsPrimary = contact.IsPrimary
                }).ToList(),*//*

                Addresses = memberDetails.Addresses?.Select(a => new AddressDTO
                {
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country
                }).ToList()
            };


            return userDetailsDTO;*/
        }
            
        

        /*public async Task<string> ChangePasswordAsync(Guid userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }


            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.CurrentPassword, user.PasswordHash))
            {
                return "Current password is incorrect.";
            }


            *//*  if (!IsValidPassword(changePasswordDTO.NewPassword))
              {
                  return "New password must be at least 6 characters long, and include one uppercase letter, one lowercase letter, one number, and one special character.";
              }*//*

            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmPassword)
            {
                return "New password and confirmation password do not match.";
            }


            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);


            var isUpdated = await _userRepository.UpdatePasswordAsync(user);

            if (!isUpdated)
            {
                return "Error updating password. Please try again.";
            }

            return "Password changed successfully.";
        }*/
        /* private bool IsValidPassword(string password)
         {
             if (password.Length < 6) return false;
             if (!password.Any(char.IsUpper)) return false;
             if (!password.Any(char.IsLower)) return false;
             if (!password.Any(char.IsDigit)) return false;
             if (!password.Any(ch => "!@#$%^&*()_+[]{}|;:',.<>?/".Contains(ch))) return false;

             return true;
         }*/
        public async Task<string> PasswordRecoveryAsync(PasswordRecoveryDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user != null)
            {

                return "Password recovery email sent.";
            }
            return "Email not found in the system.";
        }

        public async Task InvalidateTokenAsync(string token, Guid userId, DateTime expiration)
        {
            await _userRepository.InvalidateTokenAsync(token, userId, expiration);
        }

        // Method to check token validity
        public async Task<bool> IsTokenValidAsync(string token)
        {
            return await _userRepository.IsTokenValidAsync(token);
        }

        public async Task<bool> LogoutUserAsync()
        {
            try
            {
                Guid id = Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var resp = await _userRepository.InvalidateToken(id);
                if (!resp)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }

        }

        //CONTACT
        public async Task<bool> AddContactForUser(AddContactDTO addContactDTO)
        {
            // Retrieve the logged-in user's ID from the JWT token
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new FormatException("User ID claim is not in the correct format.");
            }

            // Map DTO to Contact entity
            var contact = new Contact
            {
                ContactId = Guid.NewGuid(),
                UserId = userId,
                Phone = addContactDTO.Phone
            };

            // Call the repository to save the contact
            return await _userRepository.AddContact(contact);
        }
        public async Task<bool> UpdateContactForUser(UpdateContactDTO updateContactDTO)
        {
            // Retrieve the logged-in user's ID from the JWT token
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new FormatException("User ID claim is not in the correct format.");
            }

            // Call the repository to update the contact
            var contactToUpdate = new Contact
            {
                ContactId = updateContactDTO.ContactId,
                UserId = userId,
                Phone = updateContactDTO.Phone
            };

            var result = await _userRepository.UpdateContact(contactToUpdate);

            if (!result)
            {
                throw new KeyNotFoundException("Contact not found or update failed.");
            }

            return result;
        }

        //ADDRESS
        public async Task<bool> AddAddressForUser(AddAddressDTO addAddressDTO)
        {
            
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new FormatException("User ID claim is not in the correct format.");
            }

            
            var address = new Address
            {
                AddressId = Guid.NewGuid(),
                UserId = userId,
                Street = addAddressDTO.Street,
                City = addAddressDTO.City,
                State = addAddressDTO.State,
                PostalCode = addAddressDTO.PostalCode,
                Country = addAddressDTO.Country
            };

            // Call the repository to add the address
            return await _userRepository.AddAddress(address);
        }
        public async Task<bool> UpdateAddressForUser(UpdateAddressDTO updateAddressDTO)
        {
            
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new FormatException("User ID claim is not in the correct format.");
            }

            // Create an address entity to update
            var address = new Address
            {
                AddressId = updateAddressDTO.AddressId,
                UserId = userId,
                Street = updateAddressDTO.Street,
                City = updateAddressDTO.City,
                State = updateAddressDTO.State,
                PostalCode = updateAddressDTO.PostalCode,
                Country = updateAddressDTO.Country
            };

            return await _userRepository.UpdateAddress(address);
        }

    }
}

