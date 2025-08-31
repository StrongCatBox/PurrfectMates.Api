using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // GET /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll(CancellationToken ct)
        {
            var data = await _db.Utilisateurs.AsNoTracking()
                .Select(u => new UserReadDto
                {
                    IdUtilisateur = u.IdUtilisateur,
                    nomUtilisateur = u.nomUtilisateur,
                    prenomUtilisateur = u.prenomUtilisateur,
                    emailUtilisateur = u.emailUtilisateur,
                    Role = u.Role,          // ← au lieu de x.role

                    photoProfilUtilisateur = u.photoProfilUtilisateur
                })
                .ToListAsync(ct);

            return Ok(data);
        }

        // GET /api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetOne(int id, CancellationToken ct)
        {
            var u = await _db.Utilisateurs.AsNoTracking()
                .Where(x => x.IdUtilisateur == id)
                .Select(x => new UserReadDto
                {
                    IdUtilisateur = x.IdUtilisateur,
                    nomUtilisateur = x.nomUtilisateur,
                    prenomUtilisateur = x.prenomUtilisateur,
                    emailUtilisateur = x.emailUtilisateur,
                    Role = x.Role,          // ← au lieu de x.role
                                             // ← au lieu de x.role

                    photoProfilUtilisateur = x.photoProfilUtilisateur
                })
                .FirstOrDefaultAsync(ct);

            return u is null ? NotFound() : Ok(u);
        }

        // POST /api/users
        [HttpPost]
        public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto, CancellationToken ct)
        {
            var entity = new Utilisateur
            {
                nomUtilisateur = dto.nomUtilisateur,
                prenomUtilisateur = dto.prenomUtilisateur,
                emailUtilisateur = dto.emailUtilisateur,
                motDePasseUtilisateur = dto.motDePasseUtilisateur, // ⚠️ plus tard : à hasher
                Role = dto.Role,        // ← au lieu de role = dto.role

                photoProfilUtilisateur = dto.photoProfilUtilisateur
            };

            _db.Utilisateurs.Add(entity);
            await _db.SaveChangesAsync(ct);

            var read = new UserReadDto
            {
                IdUtilisateur = entity.IdUtilisateur,
                nomUtilisateur = entity.nomUtilisateur,
                prenomUtilisateur = entity.prenomUtilisateur,
                emailUtilisateur = entity.emailUtilisateur,
                Role = entity.Role,
                photoProfilUtilisateur = entity.photoProfilUtilisateur
            };

            return CreatedAtAction(nameof(GetOne), new { id = entity.IdUtilisateur }, read);
        }

        // PUT /api/users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto, CancellationToken ct)
        {
            var u = await _db.Utilisateurs.FirstOrDefaultAsync(x => x.IdUtilisateur == id, ct);
            if (u is null) return NotFound();

            u.nomUtilisateur = dto.nomUtilisateur;
            u.prenomUtilisateur = dto.prenomUtilisateur;
            u.emailUtilisateur = dto.emailUtilisateur;
            u.Role = dto.Role;      // ← au lieu de u.role = dto.role

            u.photoProfilUtilisateur = dto.photoProfilUtilisateur;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        // DELETE /api/users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var u = await _db.Utilisateurs.FirstOrDefaultAsync(x => x.IdUtilisateur == id, ct);
            if (u is null) return NotFound();

            _db.Utilisateurs.Remove(u);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
