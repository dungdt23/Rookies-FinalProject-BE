using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.ICategoryServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
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
