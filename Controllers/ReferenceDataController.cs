using Microsoft.AspNetCore.Mvc;
using PurrfectMates.Api.Services;

namespace PurrfectMates.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceDataController : ControllerBase
    {
        private readonly ReferenceDataService _referenceDataService;

        public ReferenceDataController(ReferenceDataService referenceDataService)
        {
            _referenceDataService = referenceDataService;
        }

        [HttpGet("types-animaux")]
        public async Task<IActionResult> GetTypesAnimaux(CancellationToken ct)
        {
            var types = await _referenceDataService.GetAllTypesAnimauxAsync(ct);
            return Ok(types);
        }

        [HttpGet("tailles-animaux")]
        public async Task<IActionResult> GetTaillesAnimaux(CancellationToken ct)
        {
            var tailles = await _referenceDataService.GetAllTaillesAnimauxAsync(ct);
            return Ok(tailles);
        }

        [HttpGet("niveaux-activites")]
        public async Task<IActionResult> GetNiveauxActivites(CancellationToken ct)
        {
            var niveaux = await _referenceDataService.GetAllNiveauxActivitesAsync(ct);
            return Ok(niveaux);
        }
        [HttpGet("temperaments")]
        public async Task<IActionResult> GetTemperaments(CancellationToken ct)
        {
            var temperaments = await _referenceDataService.GetAllTemperamentsAsync(ct);
            return Ok(temperaments);
        }

        [HttpGet("types-logement")]
        public async Task<IActionResult> GetTypesLogement(CancellationToken ct)
        {
            var types = await _referenceDataService.GetAllTypesLogementAsync(ct);
            return Ok(types);
        }
    }
}