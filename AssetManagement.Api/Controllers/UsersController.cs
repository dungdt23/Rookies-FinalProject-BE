using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
	[Authorize(Roles = TypeNameContraint.TypeAdmin)]
	public async Task<IActionResult> Post([FromBody] CreateUpdateUserForm createUserForm)
	{
		//Get claim locationId from bearer token
		var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
		var token = authorizationHeader.Substring("Bearer ".Length).Trim();
		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
		var locationIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "locationId")?.Value;
		
		createUserForm.LocationId = new Guid(locationIdClaim);
		var result = await _userService.CreateAsync(createUserForm);
		if (result.StatusCode == StatusCodes.Status500InternalServerError)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, result);
		}

		return Ok(result);
	}

	[HttpPut("{id:guid}")]
	[Authorize(Roles = TypeNameContraint.TypeAdmin)]
	public async Task<IActionResult> Put(Guid id, [FromBody] CreateUpdateUserForm updateUserForm)
	{
		var result = await _userService.UpdateAsync(id, updateUserForm);
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

    [HttpGet]
    [Authorize(Roles = TypeNameContraint.TypeAdmin)]
    public async Task<IActionResult> Get([FromQuery] UserFilter filter, int index = 1, int size = 10)
    {
        var result = await _userService.GetAllAsync(filter, index, size);
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
        var result = await _userService.DisableUser(id);
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        return Ok(result);
    }

	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginForm login)
	{
		var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);
		var result = await _userService.LoginAsync(login, key);
		if (result.StatusCode == StatusCodes.Status400BadRequest)
		{
			return BadRequest(result);
		}
		return Ok(result);
	}
	[HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetById(id);
        if (result.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        return Ok(result);
    }
}