using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_6_final.Service.Abstraction;
using Project_6_final.DTOs;
namespace Project_6_final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUserService _userService;

        public ContactController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("addcontact")]
        public async Task<IActionResult> AddContact([FromBody] AddContactDTO addContactDTO)
        {
            try
            {
                var result = await _userService.AddContactForUser(addContactDTO);

                if (!result)
                {
                    return BadRequest(new { Message = "Failed to add contact. Please try again." });
                }

                return Ok(new { Message = "Contact added successfully." });
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
        [HttpPut("updatecontact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactDTO updateContactDTO)
        {
            try
            {
                var result = await _userService.UpdateContactForUser(updateContactDTO);

                if (!result)
                {
                    return BadRequest(new { Message = "Failed to update contact. Please check the details and try again." });
                }

                return Ok(new { Message = "Contact updated successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Message = "User is not logged in." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Contact not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }
    }
}
