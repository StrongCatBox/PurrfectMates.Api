using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Api.Services;
using System.Security.Claims;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Inscription
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);

            if (token == null)
            {
                return BadRequest("Email déjà utilisé");
            }

            return Ok(new { Token = token });
        }

        // Connexion
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized("Email ou mot de passe incorrect");
            }

            return Ok(new { Token = token });
        }

        // Infos utilisateur connecté
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var utilisateur = await _authService.GetMeAsync(userId);

            if (utilisateur == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                utilisateur.IdUtilisateur,
                utilisateur.nomUtilisateur,
                utilisateur.prenomUtilisateur,
                utilisateur.emailUtilisateur,
                utilisateur.Role
            });
        }
    }
}
