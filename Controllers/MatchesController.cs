using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurrfectMates.Api.Data;
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
                m.IdMatch,
                m.AnimalId,
                Animal = new
                {
                    m.Animal.nomAnimal,
                    m.Animal.race,
                    m.Animal.age,
                    m.Animal.descriptionAnimal
                },
                m.DateMatch
            });

            return Ok(result);
        }

    }
}
