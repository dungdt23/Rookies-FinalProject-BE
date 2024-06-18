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
        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
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
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
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
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
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
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
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
