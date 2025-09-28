using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext db, JwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            var emailExiste = await _db.Utilisateurs.AnyAsync(utilisateur => utilisateur.emailUtilisateur == dto.Email);
            if (emailExiste) return null;

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse);

            var utilisateur = new Utilisateur
            {
                nomUtilisateur = dto.Nom,
                prenomUtilisateur = dto.Prenom,
                emailUtilisateur = dto.Email,
                motDePasseUtilisateurHash = hash,
                Role = dto.Role,
                photoProfilUtilisateur = "default.png"
            };

            _db.Utilisateurs.Add(utilisateur);
            await _db.SaveChangesAsync();

            // ici j'appelle JwtService au lieu de générer moi-même
            return _jwtService.GenerateToken(
                utilisateur.IdUtilisateur.ToString(),
                utilisateur.emailUtilisateur
            );
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var utilisateur = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.emailUtilisateur == dto.Email);
            if (utilisateur == null) return null;

            var motDePasseOk = BCrypt.Net.BCrypt.Verify(dto.MotDePasse, utilisateur.motDePasseUtilisateurHash);
            if (!motDePasseOk) return null;

            return _jwtService.GenerateToken(
                utilisateur.IdUtilisateur.ToString(),
                utilisateur.emailUtilisateur
            );
        }

        public async Task<Utilisateur?> GetMeAsync(int userId)
        {
            return await _db.Utilisateurs.FindAsync(userId);
        }
    }
}
