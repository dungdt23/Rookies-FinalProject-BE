using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.ITypeServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services.TypeServices
{
    public class TypeService : ITypeService
    {
        private readonly IGenericRepository<Domain.Entities.Type> _typeRepository;
        private readonly IMapper _mapper;
        public TypeService(IGenericRepository<Domain.Entities.Type> typeRepository, IMapper mapper)
        {
            _typeRepository = typeRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse> GetAllAsync(int? index, int? size)
        {
            var types = await _typeRepository.GetAllAsync(index, size).ToListAsync();
            var typeDtos = _mapper.Map<IEnumerable<ResponseTypeDto>>(types);   
            return new ApiResponse{
                Data = typeDtos,
            };
        }
    }
}
