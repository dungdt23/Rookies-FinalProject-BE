using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly ILogger<AssetsController> _logger;
        public AssetsController(IAssetService assetService, ILogger<AssetsController> logger)
        {
            _assetService = assetService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> Get([FromQuery] AssetFilter filter, int index = 1, int size = 10)
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            var result = await _assetService.GetAllAsync(locationId, filter, index, size);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> Post([FromBody] RequestAssetDto requestAssetDto)
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            requestAssetDto.LocationId = locationId;
            var result = await _assetService.AddAsync(requestAssetDto);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _assetService.DeleteAsync(id);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            if (result.StatusCode == StatusCodes.Status409Conflict)
            {
                return StatusCode(StatusCodes.Status409Conflict, result);
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> Put(Guid id, [FromBody] RequestAssetDto requestAssetDto)
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            requestAssetDto.LocationId = locationId;
            var result = await _assetService.UpdateAsync(id, requestAssetDto);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _assetService.GetByIdAysnc(id);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}
