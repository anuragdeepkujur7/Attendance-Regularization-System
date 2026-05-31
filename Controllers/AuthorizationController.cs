using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_6_final.DTOs;
using Project_6_final.Service.Abstraction;


namespace Project_6_final.Controllers
{
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [Route("api/[Controller]/")]
    public class AuthorizationController : ControllerBase
    {
	//18-1
        private readonly IUserService _userService;

        public AuthorizationController(IUserService userService)
        {
            this._userService = userService;
        }
        
        [HttpPost("/login-user")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserDTO loginUser)
        {
            try
            {
                var res = await _userService.LoginUserAsync(loginUser);
                if (res == null)
                {
                    return Unauthorized("User is not authorized");
                }
                return Ok(new {Token = res } );
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            //return Ok("User authorized");
        }

        [HttpDelete("/logout-user")]
        public async Task<IActionResult> LogoutUserAsync()
        {
            try
            {
                var resp = await _userService.LogoutUserAsync();
                if (!resp)
                {
                    return BadRequest(new  { Message = "Please try again" });
                }
                return Ok(new{ Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return BadRequest(new {Message = "Please try again" });
            }
        }
    }
}
