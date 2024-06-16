using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Domain.Entities;
using AutoMapper;

namespace AssetManagement.Application.Services.AssetServices
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;
        public AssetService(IAssetRepository assetRepository,IMapper mapper)
        {
            _assetRepository = assetRepository; 
            _mapper = mapper;
        }
        public async Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssetFilter filter, int? index, int? size)
        {
            Func<Asset, object> sortConditon = x => x.AssetCode;
            switch(filter.sort)
            {
                case AssetSort.AssetName:
                    sortConditon = x => x.AssetCode;
                    break;
                case AssetSort.Category:
                    sortConditon = x => x.Category.CategoryName;
                    break;
                case AssetSort.State:
                    sortConditon = x => nameof(x.State) == nameof(AssetSort.State);
                    break;
            }
            var assets = await _assetRepository.GetAllAsync(sortConditon,filter, index, size);
            var assetDtos = _mapper.Map<IEnumerable<ResponseAssetDto>>(assets);
            var totalCount = await _assetRepository.GetTotalCountAsync(filter);
            return new PagedResponse<ResponseAssetDto>
            {
                Data = assetDtos,
                TotalCount = totalCount,
                Message = (assetDtos.Count() != 0) ? "Get asset list successfully!" : "List asset is empty",
            };
        }
    }
}
