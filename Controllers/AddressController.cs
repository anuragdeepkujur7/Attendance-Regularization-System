using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_6_final.Service.Abstraction;
using Project_6_final.DTOs;

namespace Project_6_final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IUserService _userService;

        public AddressController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("addaddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDTO addAddressDTO)
        {
            try
            {
                var result = await _userService.AddAddressForUser(addAddressDTO);

                if (!result)
                {
                    return BadRequest(new { Message = "Failed to add address. Please check the details and try again." });
                }

                return Ok(new { Message = "Address added successfully." });
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

        [HttpPut("updateaddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressDTO updateAddressDTO)
        {
            try
            {
                var result = await _userService.UpdateAddressForUser(updateAddressDTO);

                if (!result)
                {
                    return NotFound(new { Message = "Address not found or update failed." });
                }

                return Ok(new { Message = "Address updated successfully." });
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
    }
}
