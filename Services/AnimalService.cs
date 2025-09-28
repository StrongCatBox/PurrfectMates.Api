using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Services
{
    public class AnimalService
    {
        private readonly AppDbContext _db;

        public AnimalService(AppDbContext db)
        {
            _db = db;
        }

        // Récupérer tous les animaux
        public async Task<List<AnimalReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Animaux.AsNoTracking()
                .Select(animal => new AnimalReadDto
                {
                    IdAnimal = animal.IdAnimal,
                    nomAnimal = animal.nomAnimal,
                    race = animal.race,
                    age = animal.age,
                    IdUtilisateur = animal.IdUtilisateur,
                    IdNiveauActivite = animal.IdNiveauActivite,
                    IdTailleAnimal = animal.IdTailleAnimal,
                    IdTypeAnimal = animal.IdTypeAnimal,
                    descriptionAnimal = animal.descriptionAnimal
                })
                .ToListAsync(ct);
        }

        // Récupérer un animal précis
        public async Task<AnimalReadDto?> GetOneAsync(int id, CancellationToken ct)
        {
            return await _db.Animaux.AsNoTracking()
                .Where(animal => animal.IdAnimal == id)
                .Select(animal => new AnimalReadDto
                {
                    IdAnimal = animal.IdAnimal,
                    nomAnimal = animal.nomAnimal,
                    race = animal.race,
                    age = animal.age,
                    IdUtilisateur = animal.IdUtilisateur,
                    IdNiveauActivite = animal.IdNiveauActivite,
                    IdTailleAnimal = animal.IdTailleAnimal,
                    IdTypeAnimal = animal.IdTypeAnimal,
                    descriptionAnimal = animal.descriptionAnimal
                })
                .FirstOrDefaultAsync(ct);
        }

        // Créer un nouvel animal
        public async Task<AnimalReadDto> CreateAsync(int userId, AnimalCreateDto animalDto, CancellationToken ct)
        {
            var animalEntity = new Animal
            {
                nomAnimal = animalDto.nomAnimal,
                race = animalDto.race,
                age = animalDto.age,
                IdUtilisateur = userId,
                IdNiveauActivite = animalDto.IdNiveauActivite,
                IdTailleAnimal = animalDto.IdTailleAnimal,
                IdTypeAnimal = animalDto.IdTypeAnimal,
                descriptionAnimal = animalDto.descriptionAnimal
            };

            _db.Animaux.Add(animalEntity);
            await _db.SaveChangesAsync(ct);

            return new AnimalReadDto
            {
                IdAnimal = animalEntity.IdAnimal,
                nomAnimal = animalEntity.nomAnimal,
                race = animalEntity.race,
                age = animalEntity.age,
                IdUtilisateur = animalEntity.IdUtilisateur,
                IdNiveauActivite = animalEntity.IdNiveauActivite,
                IdTailleAnimal = animalEntity.IdTailleAnimal,
                IdTypeAnimal = animalEntity.IdTypeAnimal,
                descriptionAnimal = animalEntity.descriptionAnimal
            };
        }

        // Modifier un animal
        public async Task<bool> UpdateAsync(int userId, int id, AnimalUpdateDto animalDto, CancellationToken ct)
        {
            var animal = await _db.Animaux.FirstOrDefaultAsync(a => a.IdAnimal == id, ct);
            if (animal == null) return false;

            if (animal.IdUtilisateur != userId)
                throw new UnauthorizedAccessException("Vous ne pouvez modifier que vos propres animaux");

            animal.nomAnimal = animalDto.nomAnimal;
            animal.race = animalDto.race;
            animal.age = animalDto.age;
            animal.IdNiveauActivite = animalDto.IdNiveauActivite;
            animal.IdTailleAnimal = animalDto.IdTailleAnimal;
            animal.IdTypeAnimal = animalDto.IdTypeAnimal;
            animal.descriptionAnimal = animalDto.descriptionAnimal;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // Supprimer un animal
        public async Task<bool> DeleteAsync(int userId, int id, CancellationToken ct)
        {
            var animal = await _db.Animaux.FirstOrDefaultAsync(a => a.IdAnimal == id, ct);
            if (animal == null) return false;

            if (animal.IdUtilisateur != userId)
                throw new UnauthorizedAccessException("Vous ne pouvez supprimer que vos propres animaux");

            _db.Animaux.Remove(animal);
            await _db.SaveChangesAsync(ct);

            return true;
        }
    }
}
