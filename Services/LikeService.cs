using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Services
{
    public class LikeService
    {
        private readonly AppDbContext _db;

        public LikeService(AppDbContext db)
        {
            _db = db;
        }

        // Ajouter un swipe (like ou pass)
        public async Task<Like> AjouterLikeAsync(int userId, int animalId, string action)
        {
            action = action.ToLower();

            if (action != "like" && action != "pass")
                throw new Exception("Action invalide. Utilisez uniquement 'like' ou 'pass'.");

            if (await _db.Likes.AnyAsync(l => l.idUtilisateur == userId && l.idAnimal == animalId))
                throw new Exception("Vous avez déjà swipé cet animal");

            var like = new Like
            {
                idUtilisateur = userId,
                idAnimal = animalId,
                actionSwipe = action,
                dateSwipe = DateTime.UtcNow
            };

            _db.Likes.Add(like);
            await _db.SaveChangesAsync();

            // Si c’est un like → on enregistre aussi dans Matching
            if (action == "like")
            {
                var match = new Match
                {
                    UtilisateurId = userId,
                    AnimalId = animalId,
                    DateMatch = DateTime.UtcNow
                };

                _db.Matches.Add(match);
                await _db.SaveChangesAsync();
            }

            return like;
        }

        // Swipes de l’utilisateur
        public async Task<List<Like>> GetSwipesByUserAsync(int userId)
        {
            return await _db.Likes
                .Where(l => l.idUtilisateur == userId)
                .OrderByDescending(l => l.dateSwipe)
                .ToListAsync();
        }

        // Animaux likés par l’utilisateur
        public async Task<List<Match>> GetMatchesByUserAsync(int userId)
        {
            return await _db.Matches
                .Where(m => m.UtilisateurId == userId)  
                .Include(m => m.Animal)
                .ToListAsync();
        }

        // Likes reçus par les animaux d’un propriétaire
        public async Task<List<Match>> GetMatchesOnMyAnimalsAsync(int ownerId)
        {
            return await _db.Matches
                .Where(m => m.Animal.IdUtilisateur == ownerId) 
                .Include(m => m.Animal)
                .Include(m => m.Utilisateur)
                .ToListAsync();
        }
    }
}
