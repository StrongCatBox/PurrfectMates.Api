using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Api.Services;
using System.Security.Claims;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
   


        private readonly LikeService _likeService;

        public MatchesController(LikeService likeService) // injection du service
        {
            _likeService = likeService;
        }

        // Un adoptant peut voir ses matchs
        [Authorize(Roles = "Adoptant")]
        [HttpGet]
        public async Task<IActionResult> GetMyMatches()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var matches = await _likeService.GetMatchesByUserAsync(userId);

            var result = matches.Select(m => new
            {
                UtilisateurId = m.UtilisateurId,
                AnimalId = m.AnimalId,
                Animal = new
                {
                    m.Animal.nomAnimal,
                    m.Animal.race,
                    m.Animal.age,
                    m.Animal.descriptionAnimal
                },
                DateMatch = m.DateMatch
            });


            return Ok(result);
        }


        // Un propriétaire peut voir qui a liké ses animaux
       
        [Authorize(Roles = "Proprietaire")]
        [HttpGet("received")]
        public async Task<IActionResult> GetLikesOnMyAnimals()
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var matches = await _likeService.GetMatchesOnMyAnimalsAsync(ownerId);

            var result = matches.Select(m => new MatchReadDto
            {
                UtilisateurId = m.UtilisateurId,
                AnimalId = m.AnimalId,
                DateMatch = m.DateMatch,
                Animal = new AnimalDto
                {
                    NomAnimal = m.Animal.nomAnimal,
                    Race = m.Animal.race,
                    Age = m.Animal.age,
                    DescriptionAnimal = m.Animal.descriptionAnimal
                },
                Utilisateur = m.Utilisateur != null ? new UtilisateurDto
                {
                    Nom = m.Utilisateur.nomUtilisateur,
                    Prenom = m.Utilisateur.prenomUtilisateur,
                    Email = m.Utilisateur.emailUtilisateur
                } : null
            }).ToList();


            return Ok(result);
        }



    }
}
