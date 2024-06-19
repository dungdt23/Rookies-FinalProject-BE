using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.ICategoryServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace AssetManagement.Api.Controllers
{
    [Route("categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.GetAllAsync(null, null);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestCategoryDto requestCategoryDto)
        {
            var result = await _categoryService.AddAsync(requestCategoryDto);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
        [HttpGet("unique-prefix-name")]
        public async Task<IActionResult> CheckUnique([FromQuery]PrefixNameFilter prefixName)
        {
            var result = await _categoryService.IsUniqueAsync(prefixName.isPrefix, prefixName.value);
            if (result.StatusCode == StatusCodes.Status500InternalServerError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(result);
        }
    }
}
