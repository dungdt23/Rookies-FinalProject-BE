using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Diacritics.Extensions;
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
    private readonly IGenericRepository<Domain.Entities.Type> _typeRepository;
    private readonly IMapper _mapper;
    private readonly IJwtInvalidationService _jwtInvalidationService;

    public UserService(IUserRepository userRepository,
        IGenericRepository<Assignment> assignmentRepository,
        IGenericRepository<Domain.Entities.Type> typeRepository,
        IMapper mapper,
        IJwtInvalidationService jwtInvalidationService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _assignmentRepository = assignmentRepository;
        _typeRepository = typeRepository;
        _mapper = mapper;
        _jwtInvalidationService = jwtInvalidationService;
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
    public async Task<ApiResponse> CreateAsync(RequestUserCreateDto form)
    {
        var type = await _typeRepository.GetByCondition(t => t.TypeName == form.Type).AsNoTracking().FirstOrDefaultAsync();

        if (type == null)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = UserApiResponseMessageConstant.TypeNotFound,
                Data = form.Type
            };
        }

        var user = _mapper.Map<User>(form);


        user.StaffCode = _userRepository.GenerateStaffCode();
        var usernameToGenerate = $"{user.LastName.Trim()} {user.FirstName.Trim()}";
        var usernameRemovedSymbol = usernameToGenerate.RemoveDiacritics();
        user.UserName = _userRepository.GenerateUserName(usernameRemovedSymbol);
        user.TypeId = type.Id;
        user = EncryptPassword(user, $"{user.UserName}@{user.DateOfBirth:ddMMyyyy}");

        if (await _userRepository.AddAsync(user) > 0)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = UserApiResponseMessageConstant.UserCreateSuccess,
                Data = _mapper.Map<ResponseUserDto>(await _userRepository.GetByCondition(u => u.Id == user.Id)
                                                                        .Include(u => u.Type)
                                                                        .Include(u => u.Location)
                                                                        .FirstOrDefaultAsync())
            };
        }
        else
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = UserApiResponseMessageConstant.UserCreateFail,
                Data = _mapper.Map<ResponseUserDto>(user)
            };

        }
    }
    public async Task<PagedResponse<ResponseUserDto>> GetAllAsync(Guid locationId, UserFilter filter, int? index, int? size)
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
            case FieldType.Type:
                condition = x => x.Type.TypeName;
                break;
        }
        var users = await _userRepository.GetAllAsync(condition, locationId, filter, index, size);
        var userDtos = _mapper.Map<IEnumerable<ResponseUserDto>>(users);
        var totalCount = await _userRepository.GetTotalCountAsync(locationId, filter);
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
    public async Task<ApiResponse> UpdateAsync(Guid id, RequestUserEditDto form)
    {
        var type = await _typeRepository.GetByCondition(t => t.TypeName == form.Type).AsNoTracking().FirstOrDefaultAsync();

        if (type == null)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = UserApiResponseMessageConstant.UserUpdateFail,
                Data = form.Type
            };
        }

        var user = _userRepository.GetByCondition(u => u.Id == id)
                                    .Include(u => u.Type)
                                    .Include(u => u.Location)
                                    .FirstOrDefault();
        if (user == null)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Data = id,
                Message = UserApiResponseMessageConstant.UserNotFound
            };
        }

        if (type.Id != user.TypeId)
        {
            user.TokenInvalidationTimestamp = DateTime.Now;
        }

        _mapper.Map(form, user);
        user.Type = type;
        user.TypeId = type.Id;


        if (await _userRepository.UpdateAsync(user) > 0)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = UserApiResponseMessageConstant.UserUpdateSuccess,
                Data = _mapper.Map<ResponseUserDto>(user)
            };

        }
        else
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = UserApiResponseMessageConstant.UserUpdateFail,
                Data = _mapper.Map<ResponseUserDto>(user)
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
                Message = "Can't disable user because user still has valid assignments",
                StatusCode = StatusCodes.Status409Conflict
            };
    }
    public async Task<ApiResponse> LoginAsync(RequestLoginDto login, byte[] key)
    {
        var user = await _userRepository.GetByCondition(u => u.UserName == login.UserName && u.IsDeleted == false)
                                        .Include(u => u.Type)
                                        .Include(u => u.Location)
                                        .FirstOrDefaultAsync();
        if (user == null)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername,
                Data = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername
            };
        }
        var match = CheckPassword(user, login.Password);
        if (!match)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername,
                Data = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername
            };
        }


        var IsPasswordChanged = !string.Equals($"{user.UserName}@{user.DateOfBirth:ddMMyyyy}", login.Password);


        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName),
                new Claim("typeId", user.TypeId.ToString()),
                new Claim(ClaimTypes.Role, user.Type.TypeName),
                new Claim("locationId", user.LocationId.ToString()),
                new Claim("location", user.Location.LocationName),
                new Claim("BlTimestamp", DateTime.Now.ToString())
              }),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encrypterToken = tokenHandler.WriteToken(token);
        return new ApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = UserApiResponseMessageConstant.UserLoginSuccess,
            Data = new ResponseLoginDto
            {
                TokenType = "Bearer",
                Token = encrypterToken,
                IsPasswordChanged = IsPasswordChanged
            }
        };
    }
    public async Task<ApiResponse> GetById(Guid id)
    {
        var user = await _userRepository.GetByCondition(x => x.Id == id)
            .Include(x => x.Type)
            .Include(x => x.Location)
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return new ApiResponse
            {
                Message = "User doesn't exist",
                StatusCode = StatusCodes.Status404NotFound
            };
        }
        else
        {
            var userDto = _mapper.Map<ResponseUserDto>(user);
            return new ApiResponse
            {
                Data = userDto,
                Message = "Get user successfully"
            };
        }
    }
}