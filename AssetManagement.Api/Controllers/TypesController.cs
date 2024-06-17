using AssetManagement.Application.IServices.ICategoryServices;
using AssetManagement.Application.IServices.ITypeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("types")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly ITypeService _typeService;

        public TypesController(ITypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _typeService.GetAllAsync(null, null);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
    }
}
