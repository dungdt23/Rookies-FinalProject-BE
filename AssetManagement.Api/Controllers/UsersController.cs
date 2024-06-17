using System.Text;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
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
	public async Task<IActionResult> Post([FromBody] CreateUpdateUserForm createUserForm)
	{
		var result = await _userService.CreateAsync(createUserForm);
		if (result.StatusCode == StatusCodes.Status500InternalServerError)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, result);
		}

		return Ok(result);
	}

	[HttpPut("{id:guid}")]
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
}