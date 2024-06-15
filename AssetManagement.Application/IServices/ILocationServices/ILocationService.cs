using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.IServices.ILocationServices
{
    public interface ILocationService
    {
        Task<ApiResponse> GetAllAsync(int? index, int? size);

    }
}
