using PurrfectMates.Models;

namespace PurrfectMates.Api.Interfaces
{
    public interface IAnimalRepository
    {
        Task<IEnumerable<Animal>> GetAllAsync();
        Task<Animal> GetByIdAsync(int id);
        Task AddAsync(Animal animal);
        Task UpdateAsync(Animal animal);
        Task DeleteAsync(Animal animal);

    }
}
