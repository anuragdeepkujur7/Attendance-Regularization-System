using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project_6_final.DTOs;
using Project_6_final.Service.Abstraction;
using Project_6_final.Service.Implementation;
using System.Data;

namespace Project_6_final.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
   
    public class AdminController : ControllerBase
    {
       
        private readonly IUserService _userService;
        private readonly IAttendanceService _attendanceService;

        public AdminController(IUserService userService, IAttendanceService attendanceService)
        {
            
            _userService = userService;
            _attendanceService = attendanceService; 
        }

       
        [HttpPost("/user-registeration")]    // Done 22/2
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegistrationDTO userDetails)
        {
            
            try
            {
                var res = await _userService.RegisterUserAsync(userDetails);
                if (!res)
                {
                    return Conflict(new { Message = "Cannot Register User" });
                }
                return Ok("User registered successfully.");
            }
            catch (SecurityTokenException ex)   
            {
                return Unauthorized(new { Message = "Invalid token: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        
       

        [HttpGet("/getallusersdetails")]
        public async Task<IActionResult> GetAllMembers()
        {
            var res = await _userService.GetAllUserData();
            if (res == null)
            {
                return BadRequest("You need to be logged in....!");
            }
            return Ok(res);
        }

       /* [HttpGet("/get-users-attendance")]       // Done
        public async Task<IActionResult> getUsersAttendance([FromQuery] Guid id)
        {
            try
            {
                List<UserAttendanceDTO> result = await _attendanceService.GetUsersAttendance(id);
                if (result.Count == 0)
                {
                    return BadRequest( "No data found" );
                }
                else
                {
                    Console.WriteLine(result);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return BadRequest( "No data found" );

            }
        }*/

        
        // pri

        [HttpPost("/requestregularizationformember")]
        public async Task<IActionResult> RequestRegularizationForMember([FromQuery] Guid attendanceId, [FromBody] AddRegularizationDTO addRegularizationDTO)
        {
            var result = await _attendanceService.AddRegularizationRequestForMember(attendanceId, addRegularizationDTO);

            if (!result.IsLoggedIn)
            {
                return BadRequest("You need to be logged in...!");
            }

            if (result.IsSuccessful)
            {
                return Ok("Regularization Request Added Successfully");
            }

            return BadRequest("Regularization Request could not be added....!");
        }

        [HttpGet("/getallattendance")]
        public async Task<IActionResult> GetAttendaceDetails()
        {
            var res = await _attendanceService.GetAttendaceDetails();
            if (res == null)
            {
                return BadRequest("You need to be logged in....!");
            }
            return Ok(res);
        }

        
        [HttpPost("/regularizationapproval")]
        public async Task<IActionResult> RegularizationApproval([FromQuery] Guid requestId, [FromBody] AdminRegularizationDTO adminRegularizationDTO)
        {
            var result = await _attendanceService.AdminRegularization(requestId, adminRegularizationDTO);

            if (!result.IsAuthenticated)
            {
                return BadRequest("You need to be logged in...!");
            }

            if (result.IsSuccessful)
            {
                return Ok($"Regularization Status: {result.Status} Successfully");
            }

            return BadRequest("Regularization Failed");
        }

    }
}
