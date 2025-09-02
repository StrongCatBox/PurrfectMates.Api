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

        // Ajouter un swipe (like ou dislike)
        public async Task<Like> AjouterLikeAsync(int userId, int animalId, string action)
        {
            // Vérifier si un like existe déjà
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

            // Si action = "like", vérifier et créer un match
            if (action.ToLower() == "like")
            {
                var match = new Match
                {
                    UtilisateurId = userId,
                    AnimalId = animalId,
                    DateMatch = DateTime.UtcNow,
                    EstAime = true
                };

                _db.Matches.Add(match);
                await _db.SaveChangesAsync();
            }

            return like;
        }
    }
}
