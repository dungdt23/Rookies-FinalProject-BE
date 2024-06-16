using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.IServices.IAssetServices
{
    public interface IAssetService
    {
        Task<PagedResponse<ResponseAssetDto>> GetAllAsync(AssetFilter filter, int? index, int? size);
    }
}
