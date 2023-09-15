using Auth0CRUDPOC.Application_Layer;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth0CRUDPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _userService.GetUser(email);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModel userModel)
        {
            try
            {
                var user = await _userService.CreateUser(userModel);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user: {ex.Message}");
            }
        }

        [HttpPatch("{user_id}")]
        public async Task<IActionResult> UpdateUser(string user_id, UserModel userModel)
        {
            try
            {
                var user = await _userService.UpdateUser(user_id, userModel);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user: {ex.Message}");
            }
        }

        [HttpPatch("block/{user_id}")]
        public async Task<IActionResult> BlockUser(string user_id, UserModel userModel)
        {
            try
            {
                var user = await _userService.BlockUser(user_id, userModel);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user: {ex.Message}");
            }
        }

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> DeleteUser(string user_id)
        {
            try
            {
                var response = await _userService.DeleteUser(user_id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user: {ex.Message}");
            }
        }

        [HttpPost("block/{user_id}")]
        public async Task<IActionResult> BlockUser(string user_id)
        {
            try
            {
                var blockUserResponse = await _userService.BlockUser(user_id);
                if (blockUserResponse.Blocked)
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user: {ex.Message}");
            }
        }
    }
}
