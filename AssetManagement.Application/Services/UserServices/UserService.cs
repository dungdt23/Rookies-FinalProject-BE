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


    public async Task<ApiResponse> CreateAsync(CreateUserForm form)
    {
        var user = _mapper.Map<User>(form);

        user.StaffCode =  _userRepository.GenerateStaffCode();
        user.UserName =  _userRepository.GenerateUserName($"{user.FirstName.Trim()} {user.LastName.Trim()}");

        user = EncryptPassword(user, $"{user.UserName}@{user.DateOfBirth:ddMMyyyy}");

        if (await _userRepository.AddAsync(user) > 0)
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User created successfully",
                Data = user
            };
        }
        else
        {
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "There something went wrong while creating user, please try again later",
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
}
