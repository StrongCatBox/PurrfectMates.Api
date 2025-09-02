using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;
using System.Security.Claims;

namespace PurrfectMates.Api.Controllers
{
    [ApiController] // Je précise que ce contrôleur expose des routes API
    [Route("api/animals")] // Toutes mes routes commenceront par /api/animals
    public class AnimalsController : ControllerBase
    {
        private readonly AppDbContext _db;

        //  J’injecte AppDbContext pour pouvoir accéder à ma base de données
        public AnimalsController(AppDbContext db) => _db = db;

        //  Route pour récupérer la liste de tous les animaux
        // Accessible uniquement aux utilisateurs connectés (Adoptant ou Propriétaire)
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalReadDto>>> GetAll(CancellationToken ct)
        {
            //  Je récupère tous les animaux en lecture seule (AsNoTracking = plus performant)
            var data = await _db.Animaux.AsNoTracking()
                .Select(x => new AnimalReadDto
                {
                    IdAnimal = x.IdAnimal,
                    nomAnimal = x.nomAnimal,
                    race = x.race,
                    age = x.age,
                    IdUtilisateur = x.IdUtilisateur,
                    IdNiveauActivite = x.IdNiveauActivite,
                    IdTailleAnimal = x.IdTailleAnimal,
                    IdTypeAnimal = x.IdTypeAnimal,
                    descriptionAnimal = x.descriptionAnimal
                })
                .ToListAsync(ct);

            return Ok(data); //  Je retourne la liste sous forme de JSON
        }

        // Route pour récupérer un animal précis par son Id
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AnimalReadDto>> GetOne(int id, CancellationToken ct)
        {
            var a = await _db.Animaux.AsNoTracking()
                .Where(x => x.IdAnimal == id)
                .Select(x => new AnimalReadDto
                {
                    IdAnimal = x.IdAnimal,
                    nomAnimal = x.nomAnimal,
                    race = x.race,
                    age = x.age,
                    IdUtilisateur = x.IdUtilisateur,
                    IdNiveauActivite = x.IdNiveauActivite,
                    IdTailleAnimal = x.IdTailleAnimal,
                    IdTypeAnimal = x.IdTypeAnimal,
                    descriptionAnimal = x.descriptionAnimal
                })
                .FirstOrDefaultAsync(ct);

            //  Si l’animal n’existe pas → je renvoie 404 NotFound
            return a is null ? NotFound() : Ok(a);
        }

        //  Route pour créer un nouvel animal
        // Seul un utilisateur avec rôle "Proprietaire" peut ajouter un animal
        [Authorize(Roles = "Proprietaire")]
        [HttpPost]
        public async Task<ActionResult<AnimalReadDto>> Create([FromBody] AnimalCreateDto dto, CancellationToken ct)
        {
            //  Je récupère l’ID du propriétaire connecté à partir de son token JWT
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            //  Je crée une nouvelle entité Animal avec les données envoyées
            var entity = new Animal
            {
                nomAnimal = dto.nomAnimal,
                race = dto.race,
                age = dto.age,
                IdUtilisateur = userId, //  je lie automatiquement l’animal au propriétaire connecté
                IdNiveauActivite = dto.IdNiveauActivite,
                IdTailleAnimal = dto.IdTailleAnimal,
                IdTypeAnimal = dto.IdTypeAnimal,
                descriptionAnimal = dto.descriptionAnimal
            };

            _db.Animaux.Add(entity); //  J’ajoute l’animal dans le contexte EF
            await _db.SaveChangesAsync(ct); //  J’enregistre en base de données

            //  Je prépare l’objet de retour en DTO
            var read = new AnimalReadDto
            {
                IdAnimal = entity.IdAnimal,
                nomAnimal = entity.nomAnimal,
                race = entity.race,
                age = entity.age,
                IdUtilisateur = entity.IdUtilisateur,
                IdNiveauActivite = entity.IdNiveauActivite,
                IdTailleAnimal = entity.IdTailleAnimal,
                IdTypeAnimal = entity.IdTypeAnimal,
                descriptionAnimal = entity.descriptionAnimal
            };

            //  Je retourne 201 Created avec l’URL de la ressource créée
            return CreatedAtAction(nameof(GetOne), new { id = entity.IdAnimal }, read);
        }

        //  Route pour modifier un animal
        // Seul un propriétaire peut modifier SES animaux
        [Authorize(Roles = "Proprietaire")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnimalUpdateDto dto, CancellationToken ct)
        {
            var a = await _db.Animaux.FirstOrDefaultAsync(x => x.IdAnimal == id, ct);
            if (a is null) return NotFound(); //  Si l’animal n’existe pas → 404

            //  Je vérifie que l’animal appartient bien au propriétaire connecté
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (a.IdUtilisateur != userId)
                return Forbid("Vous ne pouvez modifier que vos propres animaux");

            //  Je mets à jour les propriétés
            a.nomAnimal = dto.nomAnimal;
            a.race = dto.race;
            a.age = dto.age;
            a.IdNiveauActivite = dto.IdNiveauActivite;
            a.IdTailleAnimal = dto.IdTailleAnimal;
            a.IdTypeAnimal = dto.IdTypeAnimal;
            a.descriptionAnimal = dto.descriptionAnimal;

            await _db.SaveChangesAsync(ct); //  Sauvegarde en base
            return NoContent(); //  Je retourne 204 NoContent (succès sans contenu)
        }

        //  Route pour supprimer un animal
        // Seul un propriétaire peut supprimer SES animaux
        [Authorize(Roles = "Proprietaire")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var a = await _db.Animaux.FirstOrDefaultAsync(x => x.IdAnimal == id, ct);
            if (a is null) return NotFound(); //  Si l’animal n’existe pas → erreur 404

            //  Vérification : l’animal doit appartenir au propriétaire connecté
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (a.IdUtilisateur != userId)
                return Forbid("Vous ne pouvez supprimer que vos propres animaux");

            _db.Animaux.Remove(a); //  Je supprime l’animal
            await _db.SaveChangesAsync(ct); //  Je valide en base
            return NoContent(); //  Succès
        }
    }
}
