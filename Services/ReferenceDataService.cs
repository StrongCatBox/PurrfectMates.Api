using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Services
{
    public class ReferenceDataService
    {
        private readonly AppDbContext _context;

        public ReferenceDataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TypeAnimal>> GetAllTypesAnimauxAsync(CancellationToken ct)
        {
            return await _context.TypesAnimaux.ToListAsync(ct);
        }

        public async Task<List<TailleAnimal>> GetAllTaillesAnimauxAsync(CancellationToken ct)
        {
            return await _context.TaillesAnimaux.ToListAsync(ct);
        }

        public async Task<List<NiveauActivite>> GetAllNiveauxActivitesAsync(CancellationToken ct)
        {
            return await _context.NiveauxActivites.ToListAsync(ct);
        }
        public async Task<List<Temperament>> GetAllTemperamentsAsync(CancellationToken ct)
        {
            return await _context.Temperaments.ToListAsync(ct);
        }

        public async Task<List<TypeLogement>> GetAllTypesLogementAsync(CancellationToken ct)
        {
            return await _context.TypesLogements.ToListAsync(ct);
        }
    }
}