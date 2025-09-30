using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Interfaces;
using PurrfectMates.Models;


namespace PurrfectMates.Api.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {

        private readonly AppDbContext _db;

        public AnimalRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Animal>> GetAllAsync()
        {
            return await _db.Animaux.AsNoTracking().ToListAsync();
        }
        public async Task<Animal?> GetByIdAsync(int id)
        {
            return await _db.Animaux.FindAsync(id);
        }

        public async Task AddAsync(Animal animal)
        {
            _db.Animaux.Add(animal);
        }

        public async Task UpdateAsync(Animal animal)
        {
            _db.Animaux.Update(animal);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Animal animal)
        {
            _db.Animaux.Remove(animal);
            await _db.SaveChangesAsync();
        }
    }
}
