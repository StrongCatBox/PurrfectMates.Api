using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Models;

namespace PurrfectMates.Api.Services
{
    public class LikeService
    {
        private readonly AppDbContext _db;

        // J’injecte le DbContext pour pouvoir accéder à la base de données
        public LikeService(AppDbContext db)
        {
            _db = db;
        }

        // Cette méthode ajoute un swipe (soit un "like", soit un "pass")
        public async Task<Like> AjouterLikeAsync(int userId, int animalId, string action)
        {
            // Je normalise l’action (tout en minuscules)
            action = action.ToLower();

            // Je vérifie que l’action est bien "like" ou "pass"
            if (action != "like" && action != "pass")
                throw new Exception("Action invalide. Utilisez uniquement 'like' ou 'pass'.");

            // Je vérifie si cet utilisateur a déjà swipé cet animal
            if (await _db.Likes.AnyAsync(l => l.idUtilisateur == userId && l.idAnimal == animalId))
                throw new Exception("Vous avez déjà swipé cet animal");

            // Je crée un objet Like qui sera inséré dans la table Swipe
            var like = new Like
            {
                idUtilisateur = userId,
                idAnimal = animalId,
                actionSwipe = action,        // ici j’enregistre "like" ou "pass"
                dateSwipe = DateTime.UtcNow // je stocke la date du swipe
            };

            // J’ajoute le Like dans la base
            _db.Likes.Add(like);
            await _db.SaveChangesAsync();

            // Si c’est un "like", je crée aussi une entrée dans la table Matching
            if (action == "like")
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

            // Je retourne le Like créé (utile pour le contrôleur / Swagger)
            return like;
        }

        public async Task<List<Like>> GetSwipesByUserAsync(int userId)
        {
            return await _db.Likes
                .Where(l => l.idUtilisateur == userId)
                .OrderByDescending(l => l.dateSwipe)
                .ToListAsync();
        }

        public async Task<List<Match>> GetMatchesByUserAsync(int userId)
        {
            return await _db.Matches
                .Where(m => m.UtilisateurId == userId && m.EstAime == true)
                .Include(m => m.Animal) // pour récupérer les infos de l’animal
                .ToListAsync();
        }


    }
}
