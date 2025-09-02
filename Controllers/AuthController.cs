using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;


namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        //  Inscription
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Utilisateurs.AnyAsync(u => u.emailUtilisateur == dto.Email))
                return BadRequest("Email déjà utilisé");

            // Hash du mot de passe
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse);

            var user = new Utilisateur
            {
                nomUtilisateur = dto.Nom,
                prenomUtilisateur = dto.Prenom,
                emailUtilisateur = dto.Email,
                motDePasseUtilisateurHash = hash,
                Role = dto.Role,
                photoProfilUtilisateur = null
            };

            _db.Utilisateurs.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { Token = GenererJwt(user) });
        }

        // Connexion
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Utilisateurs
                .FirstOrDefaultAsync(u => u.emailUtilisateur == dto.Email);

            if (user == null) return Unauthorized("Email invalide");

            if (!BCrypt.Net.BCrypt.Verify(dto.MotDePasse, user.motDePasseUtilisateurHash))
                return Unauthorized("Mot de passe incorrect");

            return Ok(new { Token = GenererJwt(user) });
        }

        // Infos utilisateur connecté
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.Utilisateurs.FindAsync(int.Parse(userId));

            return Ok(new
            {
                user!.IdUtilisateur,
                user.nomUtilisateur,
                user.prenomUtilisateur,
                user.emailUtilisateur,
                user.Role
            });
        }

        // Génération JWT
        private string GenererJwt(Utilisateur user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUtilisateur.ToString()),
                new Claim(ClaimTypes.Email, user.emailUtilisateur),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
