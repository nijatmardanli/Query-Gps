using Microsoft.AspNetCore.Mvc;
using Query_Gps.WebApi.Dtos;
using Query_Gps.WebApi.Services.Abstract;

namespace Query_Gps.WebApi.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] RegionGetDto regionGetDto)
        {
            var result = await _regionService.FindAsync(regionGetDto);
            return Ok(result);
        }

        [HttpPost("Syncronize")]
        public async Task<IActionResult> SyncronizeAsync()
        {
            await _regionService.SyncronizeAsync();

            return Ok();
        }
    }
}
