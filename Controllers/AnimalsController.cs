using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AnimalsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalReadDto>>> GetAll(CancellationToken ct)
        {
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

            return Ok(data);
        }


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

            return a is null ? NotFound() : Ok(a);
        }

        [HttpPost]
        public async Task<ActionResult<AnimalReadDto>> Create([FromBody] AnimalCreateDto dto, CancellationToken ct)
        {
            var entity = new Animal
            {
                nomAnimal = dto.nomAnimal,
                race = dto.race,
                age = dto.age,
                IdUtilisateur = dto.IdUtilisateur,
                IdNiveauActivite = dto.IdNiveauActivite,
                IdTailleAnimal = dto.IdTailleAnimal,
                IdTypeAnimal = dto.IdTypeAnimal,
                descriptionAnimal = dto.descriptionAnimal
            };

            _db.Animaux.Add(entity);
            await _db.SaveChangesAsync(ct);

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

            return CreatedAtAction(nameof(GetOne), new { id = entity.IdAnimal }, read);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnimalUpdateDto dto, CancellationToken ct)
        {
            var a = await _db.Animaux.FirstOrDefaultAsync(x => x.IdAnimal == id, ct);
            if (a is null) return NotFound();

            a.nomAnimal = dto.nomAnimal;
            a.race = dto.race;
            a.age = dto.age;
            a.IdNiveauActivite = dto.IdNiveauActivite;
            a.IdTailleAnimal = dto.IdTailleAnimal;
            a.IdTypeAnimal = dto.IdTypeAnimal;
            a.descriptionAnimal = dto.descriptionAnimal;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var a = await _db.Animaux.FirstOrDefaultAsync(x => x.IdAnimal == id, ct);
            if (a is null) return NotFound();

            _db.Animaux.Remove(a);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
