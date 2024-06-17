using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AssetFilter filter, int index = 1, int size = 10)
        {
            var result = await _assetService.GetAllAsync(filter, index,  size);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestAssetDto requestAssetDto)
        {
            var result = await _assetService.AddAsync(requestAssetDto);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _assetService.DeleteAsync(id);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] RequestAssetDto requestAssetDto)
        {
            var result = await _assetService.UpdateAsync(id,requestAssetDto);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            return Ok(result);
        }
    }
}
