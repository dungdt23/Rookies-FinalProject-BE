using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.ICategoryServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AssetManagement.Application.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> AddAsync(RequestCategoryDto requestCategoryDto)
        {
            var category = _mapper.Map<Category>(requestCategoryDto);
            var status = await _categoryRepository.AddAsync(category);
            if (status == StatusConstant.Success)
            {
                return new ApiResponse
                {
                    Data = category,
                    Message = "Add new category successfully!"
                };
            }
            else
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Add new category failed!"
                };
            }
        }
        public async Task<ApiResponse> IsUniqueAsync(bool isPrefix, string value)
        {
            var message = string.Empty;
            if (isPrefix)
            {
                var duplicatedPrefix = await _categoryRepository
                .GetByCondition(x => x.Prefix.ToLower().Equals(value.ToLower()))
                .FirstOrDefaultAsync();
                message = (duplicatedPrefix != null) ? "Category prefix is duplicated!" : string.Empty;
                return new ApiResponse
                {
                    Data = string.IsNullOrEmpty(message) ? true : false,
                    Message = message
                };
            }
            else
            {
                var duplicatedName = await _categoryRepository
                .GetByCondition(x => x.CategoryName.ToLower().Equals(value.ToLower()))
                .FirstOrDefaultAsync();
                message = (duplicatedName != null) ? "Category name is duplicated!" : string.Empty;
                return new ApiResponse
                {
                    Data = string.IsNullOrEmpty(message) ? true : false,
                    Message = message
                };
            }
        }
        public async Task<ApiResponse> GetAllAsync(int? index, int? size)
        {
            var categories = await _categoryRepository.GetAllAsync(index, size)
                .OrderBy(x => x.CategoryName)
                .ToListAsync();
            var categoryDtos = _mapper.Map<IEnumerable<ResponseCategoryDto>>(categories);
            return new ApiResponse
            {
                Data = categoryDtos,
            };
        }
    }
}
