using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_6_final.DTOs;
using Project_6_final.Service.Abstraction;
using Project_6_final.Service.Implementation;
using System.Data;
using System.Security.Claims;

namespace Project_6_final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttendanceService _attendanceService;
        public UsersController(IUserService userService)
        {
            _userService = userService;

        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDto)
        {
             try
              {
                  var user = await _userService.CreateUserAsync(userDto);
                  // return Ok(new { message = "User registered successfully", data = user });
                  return Ok(user);

              }
              catch (InvalidOperationException ex)
              {
                  // return BadRequest(new { error = ex.Message });
                  return BadRequest(ex.Message);
              };
        }
        
        [HttpGet("profile")]
        
        public async Task<ActionResult> ViewProfile()
        {
             try
             {
                 var userProfile = await _userService.GetUserProfile();

                 if (userProfile == null)
                 {
                     return NotFound(new { Message = "User profile not found." });
                 }

                 return Ok(userProfile);
             }
             catch (UnauthorizedAccessException)
             {
                 return Unauthorized(new { Message = "User is not logged in." });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
             }
            
        }

        [HttpPut("updateprofile")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile([FromBody] UserUpdateDTO profileUpdateDTO)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userService.UpdateUserProfileAsync(userId, profileUpdateDTO);

            return Ok(new { message = "Updated" });
        }



        [HttpPost("changepassword")]
        [Authorize]
        /*public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);

            if (result == "Password changed successfully.")
            {
                return Ok(new { Message = result });
            }

            return BadRequest(new { Message = result });
        }*/


        [HttpPost("password-recovery")]
        public async Task<IActionResult> PasswordRecovery([FromBody] PasswordRecoveryDTO dto)
        {
            var message = await _userService.PasswordRecoveryAsync(dto);
            return Ok(new { Message = message });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("User is not logged in.");
            }

            var expiration = DateTime.UtcNow.AddMinutes(30);
            await _userService.InvalidateTokenAsync(token, Guid.Parse(userId), expiration);

            return Ok("Logged out successfully.");
        }

       
    }
}
