using Microsoft.AspNetCore.Mvc;
using PurrfectMates.Api.Services;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            
            if (request.Email == "test@demo.com" && request.Password == "123456")
            {
                var token = _jwtService.GenerateToken("1", request.Email);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
