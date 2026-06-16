using Microsoft.AspNetCore.Mvc;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TEMP SIMPLE LOGIN (replace later with DB)
            if (request.Username == "admin" && request.Password == "1234")
            {
                return Ok(new
                {
                    Username = request.Username,
                    Role = "Admin"
                });
            }

            return Unauthorized("Invalid credentials");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}