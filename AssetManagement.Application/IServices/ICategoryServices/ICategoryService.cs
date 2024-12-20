﻿using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;

namespace AssetManagement.Application.IServices.ICategoryServices
{
    public interface ICategoryService
    {
        Task<ApiResponse> GetAllAsync(int? index, int? size);
        Task<ApiResponse> AddAsync(RequestCategoryDto requestCategoryDto);
        Task<ApiResponse> IsUniqueAsync(bool isPrefix, string value);

    }
}
