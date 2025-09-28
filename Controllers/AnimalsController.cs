using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectMates.Api.Dtos;
using PurrfectMates.Api.Services;
using System.Security.Claims;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalService _animalService;

        // J’injecte mon service métier (au lieu de DbContext direct)
        public AnimalsController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var animals = await _animalService.GetAllAsync(ct);
            return Ok(animals);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id, CancellationToken ct)
        {
            var animal = await _animalService.GetOneAsync(id, ct);

            if (animal == null)
                return NotFound();
            else
                return Ok(animal);
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnimalCreateDto animalDto, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var created = await _animalService.CreateAsync(userId, animalDto, ct);

            return CreatedAtAction(nameof(GetOne), new { id = created.IdAnimal }, created);
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnimalUpdateDto animalDto, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var success = await _animalService.UpdateAsync(userId, id, animalDto, ct);

                if (!success) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize(Roles = "Proprietaire")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var success = await _animalService.DeleteAsync(userId, id, ct);

                if (!success) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
