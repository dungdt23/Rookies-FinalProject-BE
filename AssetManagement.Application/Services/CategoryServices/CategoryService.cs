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

        public async Task<ApiResponse> GetAllAsync(int? index, int? size)
        {
            var categories = await _categoryRepository.GetAllAsync(index, size).ToListAsync();
            var categoryDtos = _mapper.Map<IEnumerable<ResponseCategoryDto>>(categories);
            return new ApiResponse
            {
                Data = categoryDtos,
            };
        }
    }
}
