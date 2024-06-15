using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.ILocationServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services.LocationServices
{
    public class LocationService : ILocationService
    {
        private readonly IGenericRepository<Location> _locationRepository;
        private readonly IMapper _mapper;
        public LocationService(IGenericRepository<Location> locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse> GetAllAsync(int? index, int? size)
        {
            var locations = await _locationRepository.GetAllAsync(index, size).ToListAsync();
            var locationDtos = _mapper.Map<IEnumerable<ResponseLocationDto>>(locations);
            return new ApiResponse { 
                Data = locationDtos
            };
        }
    }
}
