using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AssetManagement.Application.Services.UserServices;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly IGenericRepository<Assignment> _assignmentRepository;
	private readonly IMapper _mapper;
	public UserService(IUserRepository userRepository, IGenericRepository<Assignment> assignmentRepository, IMapper mapper)
	{
		_userRepository = userRepository;
		_mapper = mapper;
		_assignmentRepository = assignmentRepository;
		_mapper = mapper;
	}

	public bool CheckPassword(User user, string password)
	{
		bool result;
		using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
		{
			var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			result = compute.SequenceEqual(user.PasswordHash);
		}
		return result;
	}


	public async Task<ApiResponse> CreateAsync(CreateUpdateUserForm form)
	{
		var user = _mapper.Map<User>(form);

		user.StaffCode = _userRepository.GenerateStaffCode();
		user.UserName = _userRepository.GenerateUserName($"{user.FirstName.Trim()} {user.LastName.Trim()}");
		user = EncryptPassword(user, $"{user.UserName}@{user.DateOfBirth:ddMMyyyy}");

		if (await _userRepository.AddAsync(user) > 0)
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status200OK,
				Message = UserApiResponseMessageContraint.UserCreateSuccess,
				Data = user
			};
		}
		else
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status500InternalServerError,
				Message = UserApiResponseMessageContraint.UserCreateFail,
				Data = user
			};

		}
	}
	public async Task<PagedResponse<ResponseUserDto>> GetAllAsync(UserFilter filter, int? index, int? size)
	{
		Func<User, object> condition = x => x.StaffCode;
		switch (filter.FieldFilter)
		{
			case FieldType.FullName:
				condition = x => (x.FirstName + " " + x.LastName);
				break;
			case FieldType.JoinedDate:
				condition = x => x.JoinedDate;
				break;
		}
		var users = await _userRepository.GetAllAsync(condition, filter, index, size);
		var userDtos = _mapper.Map<IEnumerable<ResponseUserDto>>(users);
		var totalCount = await _userRepository.GetTotalCountAsync(filter);
		return new PagedResponse<ResponseUserDto>
		{
			Data = userDtos,
			Message = (userDtos.Count() != 0) ? "Get user list successfully!" : "List user is empty",
			TotalCount = totalCount
		};
	}

	public User EncryptPassword(User user, string password)
	{
		using (HMACSHA512? hmac = new HMACSHA512())
		{
			user.PasswordSalt = hmac.Key;
			user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		}
		return user;
	}

	public async Task<ApiResponse> UpdateAsync(Guid id, CreateUpdateUserForm form)
	{
		var user = _userRepository.GetByCondition(u => u.Id == id).FirstOrDefault();
		if (user == null)
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status404NotFound,
				Data = id,
				Message = UserApiResponseMessageContraint.UserNotFound
			};
		}

		_mapper.Map(form, user);

		if (await _userRepository.UpdateAsync(user) > 0)
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status200OK,
				Data = user,
				Message = UserApiResponseMessageContraint.UserUpdateSuccess
			};

		}
		else
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status500InternalServerError,
				Data = user,
				Message = UserApiResponseMessageContraint.UserUpdateFail,
			};
		}
	}
	public async Task<ApiResponse> DisableUser(Guid id)
	{
		var user = await _userRepository.GetByCondition(x => x.Id == id && !x.IsDeleted)
			.Include(x => x.ReceivedAssignments).FirstOrDefaultAsync();
		if (user == null) return new ApiResponse
		{
			Message = "User not found or no long active!",
			StatusCode = StatusCodes.Status500InternalServerError
		};
		var userValidAssignment = user.ReceivedAssignments
			.Where(x => x.State != Domain.Enums.TypeAssignmentState.Rejected && !x.IsDeleted);
		//check if user have any valid asignment
		if (userValidAssignment.Count() == 0)
		{
			await _userRepository.DeleteAsync(id);
			return new ApiResponse
			{
				Message = "Disable user successfully!"
			};
		}
		else
			return new ApiResponse
			{
				Message = "Can't disable user because user still has valid assignments"
			};
	}


	public async Task<ApiResponse> LoginAsync(LoginForm login, byte[] key)
	{
		var user = await _userRepository.GetByCondition(u => u.UserName == login.UserName)
										.Include(u => u.Type)
										.Include(u => u.Location)
										.FirstOrDefaultAsync();
		if (user == null)
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status400BadRequest,
				Message = UserApiResponseMessageContraint.UserLoginWrongPasswordOrUsername,
				Data = UserApiResponseMessageContraint.UserLoginWrongPasswordOrUsername
			};
		}
		var match = CheckPassword(user, login.Password);
		if (!match)
		{
			return new ApiResponse
			{
				StatusCode = StatusCodes.Status400BadRequest,
				Message = UserApiResponseMessageContraint.UserLoginWrongPasswordOrUsername,
				Data = UserApiResponseMessageContraint.UserLoginWrongPasswordOrUsername
			};
		}
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim("id", user.Id.ToString()),
				new Claim("username", user.UserName),
				new Claim("typeId", user.TypeId.ToString()),
				new Claim("type", user.Type.TypeName),
				new Claim("locationId", user.LocationId.ToString()),
				new Claim("location", user.Location.LocationName)
			  }),
			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		var encrypterToken = tokenHandler.WriteToken(token);
		return new ApiResponse
		{
			StatusCode = StatusCodes.Status200OK,
			Message = UserApiResponseMessageContraint.UserLoginSuccess,
			Data = new
			{
				tokenType = "Bearer",
				token = encrypterToken
			}

		};
	}

}
