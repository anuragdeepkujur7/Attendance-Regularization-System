using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project_6_final.DTOs;
using Project_6_final.Models;
using Project_6_final.Service.Abstraction;
using Project_6_final.Service.Implementation;
using System.Security.Claims;

namespace Project_6_final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
       


        [HttpPost]
        [Route("/markattendance")]
        [Authorize] 
        public async Task<IActionResult> AddAttendance()
        { 
            try
            {
                bool isAdded = await _attendanceService.AddMemberAttendance();  //in use
                if (isAdded)
                {
                    return Ok("Attendance Added Successfully");
                }
                return BadRequest("Attendance could not be added....!");
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("You need to be logged in...!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
         

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsersAttendance()
        {
            var attendance = await _attendanceService.GetAllUsersAttendanceAsync();
            return Ok(attendance);
        }
       /* [HttpGet("my-attendance")]
        [Authorize(Roles = "User,Admin")]*/


        [HttpGet("/getmyattendance")]
        public async Task<IActionResult> GetAttendance()
        {
            var res = await _attendanceService.GetMemberAttendance();
            if (res == null)
            {
                return BadRequest("No attendance Data was found...!");
            }
            return Ok(res);
        }
        

        [Authorize(Roles = "User")]
        [HttpPost("/requestregularization")]
        public async Task<IActionResult> RequestRegularization([FromQuery] Guid attendanceId, [FromBody] AddRegularizationDTO addRegularizationDTO)
        {
            var result = await _attendanceService.AddMemberRegularizationRequest(attendanceId, addRegularizationDTO);

            if (!result)
            {
                return BadRequest("You need to be logged in or the request could not be processed.");
            }

            return Ok("Regularization Request Added Successfully");
        }
        //ch
        [HttpGet("/getmonthlyattendancereport")]
        [Authorize]
        public async Task<IActionResult> GetUserAttendanceReport([FromQuery] UserAttendanceReportDTO userAttendanceReportDTO)
        {
            

            var res = await _attendanceService.GetMemberAttendanceReportUser(userAttendanceReportDTO);
            if (res == null)
            {
                return NotFound(new { Message = "No data was found for the entered details." });
            }
            return Ok(res);
        }
    }
}

