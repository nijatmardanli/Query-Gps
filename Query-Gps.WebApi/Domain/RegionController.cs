using Microsoft.AspNetCore.Mvc;
using Query_Gps.WebApi.Services.Abstract;

namespace Query_Gps.WebApi.Domain
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpPost("Syncronize")]
        public async Task<IActionResult> Syncronize()
        {
            await _regionService.SyncronizeAsync();

            return Ok();
        }
    }
}
