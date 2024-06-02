using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Mvc;
using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Dtos;
using Query_Gps.WebApi.Repositories.Abstract;
using Query_Gps.WebApi.Services.Abstract;

namespace Query_Gps.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IDriverRepository _driverRepository;

        public DriverController(IDriverService driverService, IDriverRepository driverRepository)
        {
            _driverService = driverService;
            _driverRepository = driverRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] DriverSearchDto driverSearchDto)
        {
            var result = await _driverService.FindAsync(driverSearchDto);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] DriverAddDto driverAddDto)
        {
            var result = await _driverService.AddAsync(driverAddDto);

            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> PutAsync([FromBody] DriverUpdateCoordinateDto driverUpdateCoordinateDto)
        {
            var result = await _driverService.UpdateCoordinateAsync(driverUpdateCoordinateDto);

            return Ok(result);
        }
    }
}
