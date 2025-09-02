using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("{animalId:int}")]
        public async Task<IActionResult> Swipe(int animalId, [FromQuery] string action)
        {
            try
            {
                // Je récupère l'utilisateur connecté depuis le token
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // J'appelle la logique métier dans LikeService
                var like = await _likeService.AjouterLikeAsync(userId, animalId, action);

                return Ok(new
                {
                    Message = "Swipe enregistré",
                    like.IdSwipe,
                    like.actionSwipe,
                    like.dateSwipe
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erreur = ex.Message });
            }
        }
    }
}
