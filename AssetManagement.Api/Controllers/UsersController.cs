using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace AssetManagement.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppSetting _applicationSettings;
    public UsersController(IUserService userService, IOptions<AppSetting> applicationSettings)
    {
        _userService = userService;
        _applicationSettings = applicationSettings.Value;
    }

    [HttpPost]
    [Authorize(Roles = TypeNameConstants.TypeAdmin)]
    public async Task<IActionResult> Post([FromBody] RequestUserCreateDto createUserForm)
    {
        var locationIdClaim = HttpContext.GetClaim("locationId");
        Guid locationIdGuid;
        if (!Guid.TryParse(locationIdClaim, out locationIdGuid))
        {
            return Unauthorized();
        }
        createUserForm.LocationId = locationIdGuid;
        var result = await _userService.CreateAsync(createUserForm);
        if(result.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(result);
        }
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = TypeNameConstants.TypeAdmin)]
    public async Task<IActionResult> Put(Guid id, [FromBody] RequestUserEditDto updateUserForm)
    {
        var result = await _userService.UpdateAsync(id, updateUserForm);
        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(result);
        }
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = TypeNameConstants.TypeAdmin)]
    public async Task<IActionResult> Get([FromQuery] UserFilter filter, int index = 1, int size = 10)
    {
        var locationIdClaim = HttpContext.GetClaim("locationId");
        var locationId = new Guid(locationIdClaim);
        var result = await _userService.GetAllAsync(locationId, filter, index, size);
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
        var userIdClaim = HttpContext.GetClaim("id");
        var userId = new Guid(userIdClaim);
        if (userId == id)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        var result = await _userService.DisableUser(id);
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }
        if (result.StatusCode == StatusCodes.Status409Conflict)
        {
            return Conflict();
        }
        return Ok(result);
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] RequestLoginDto login)
    {
        var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);
        var duration = _applicationSettings.Duration;
        var result = await _userService.LoginAsync(login, key,duration);
        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(result);
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Forbid();
        }
        return Ok(result);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = TypeNameConstants.TypeAdmin)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetById(id);
		if (result.StatusCode == StatusCodes.Status404NotFound)
		{
			return NotFound(result);
		}
		if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        return Ok(result);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] RequestChangePasswordDto request)
    {
        var userIdClaim = HttpContext.GetClaim("id");
        Guid userIdGuid;
        if (!Guid.TryParse(userIdClaim, out userIdGuid))
        {
            return Unauthorized();
        }
        var result = await _userService.ChangePasswordAsync(request.OldPassword, request.NewPassword, userIdGuid, false);
        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(result);
        }
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        return Ok(result);
    }

    [HttpPut("change-password-first-time")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordFirstTime([FromBody] RequestChangePasswordFirstTimeDto request)
    {
        var userIdClaim = HttpContext.GetClaim("id");
        Guid userIdGuid;
        if (!Guid.TryParse(userIdClaim, out userIdGuid))
        {
            return Unauthorized();
        }
        var result = await _userService.ChangePasswordAsync(null, request.NewPassword, userIdGuid, true);
        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(result);
        }
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        return Ok(result);
    }
}