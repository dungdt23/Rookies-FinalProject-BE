using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.IServices.ITypeServices
{
    public interface ITypeService
    {
        Task<ApiResponse> GetAllAsync(int? index, int? size);
    }
}
