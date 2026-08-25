using lofi_backend.Models;
using lofi_backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllUsers()
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            UserData currentUser;
            try
            {
                currentUser = _service.GetUserById(currentUserId);
            }
            catch (Exception)
            {
                return Forbid();
            }

            if (!currentUser.IsAdmin)
            {
                return Forbid();
            }
            try
            {
                var result = _service.GetAllUsers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [Authorize]
        [Route("me")]
        [HttpGet]
        public IActionResult GetUserAsync()
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            try
            {
                var result = _service.GetUserById(currentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserWithPassword user)
        {
            try
            {
                var newUser = await _service.CreateUser(user);
                return Ok(newUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult EditUser([FromBody] UserData user)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            if (user.Id != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var existingUser = _service.GetUserById(currentUserId);
                user.IsAdmin = existingUser.IsAdmin;

                var result = _service.EditUser(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult RemoveUser(string id)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            UserData currentUser;
            try
            {
                currentUser = _service.GetUserById(currentUserId);
            }
            catch (Exception)
            {
                return Forbid();
            }

            if (!currentUser.IsAdmin)
            {
                return Forbid();
            }

            if (id == "")
            {
                return BadRequest("User id must be greater than zero.");
            }

            UserData removeUser = _service.RemoveUser(id);


            if (removeUser == null)
            {
                return NotFound($"User with id {id} was not found.");

            }
            return NoContent();
        }

    }
}
