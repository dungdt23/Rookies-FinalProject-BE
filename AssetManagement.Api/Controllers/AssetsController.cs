using AssetManagement.Api.Hubs;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AssetManagement.Api.Controllers
{
    [Route("assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly ILogger<AssetsController> _logger;
        private readonly IHubContext<SignalRHub> _hubContext;
        public AssetsController(IAssetService assetService, ILogger<AssetsController> logger, IHubContext<SignalRHub> hubContext)
        {
            _assetService = assetService;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> Get([FromQuery] AssetFilter filter, int index = PaginationConstant.DefaultIndex, int size = PaginationConstant.DefaultSize)
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
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
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
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _assetService.DeleteAsync(id);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            if (result.StatusCode == StatusCodes.Status409Conflict)
            {
                return Conflict(result);
            }
            await _hubContext.Clients.All.SendAsync("Deleted", id);
            return Ok(result);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
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
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
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
