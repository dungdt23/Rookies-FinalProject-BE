using System.Security.Cryptography;
using System.Text;
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

namespace AssetManagement.Application.Services.UserServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
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
    public async Task<ApiResponse> GetAllAsync(UserFilter filter, int? index, int? size)
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
        return new ApiResponse
        {
            Data = userDtos,
            Message = (userDtos.Count() != 0) ? "Get user list successfully!" : "List user is empty"
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
}
