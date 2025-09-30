using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Api.Services;
using PurrfectMates.Models;
using System.Security.Claims;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/likes")]
    public class LikesController : ControllerBase
    {
        private readonly LikeService _likeService;

        public LikesController(LikeService likeService)
        {
            _likeService = likeService;
        }

        // Un adoptant peut liker ou disliker un animal
        [Authorize(Roles = "Adoptant")]
        [HttpPost("{idAnimal}")]
        public async Task<IActionResult> Swipe(int idAnimal, [FromQuery] string action)
        {
            try
            {
                // Je récupère l'utilisateur connecté depuis le token
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // J'appelle la logique métier dans LikeService
                var like = await _likeService.AjouterLikeAsync(userId, idAnimal, action);

                return Ok(new LikeReadDto
                {
                    IdSwipe = like.IdSwipe,
                    AnimalId = like.idAnimal,
                    ActionSwipe = like.actionSwipe,
                    DateSwipe = like.dateSwipe
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new { Erreur = ex.Message });
            }
        }


        // Récupérer l’historique des swipes d’un utilisateur
        [Authorize(Roles = "Adoptant")]
        [HttpGet]
        public async Task<IActionResult> GetMySwipes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var swipes = await _likeService.GetSwipesByUserAsync(userId);

            return Ok(swipes.Select(s => new {
                s.idAnimal,
                s.actionSwipe,
                s.dateSwipe
            }));
        }

    }
}
