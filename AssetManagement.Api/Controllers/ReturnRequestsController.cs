using AssetManagement.Application.Dtos.Common;
using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.IServices;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("return-requests")]
    [ApiController]
    public class ReturnRequestsController : ControllerBase
    {
        private readonly IReturnRequestService _returnRequestService;

        public ReturnRequestsController(IReturnRequestService returnRequestService)
        {
            _returnRequestService = returnRequestService;
        }

        [HttpGet]
        [Authorize(Roles = TypeNameContraint.TypeAdmin)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllReturnRequest request)
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            var (returnRequests, totalCount) = await _returnRequestService.GetAllReturnRequestAsync(request, locationId);

            PaginatedResult<ReturnRequestGetAllViewModel> paginateResult = new PaginatedResult<ReturnRequestGetAllViewModel>
            {
                Data = returnRequests,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PerPage
            };

            return Ok(paginateResult);
        }
    }
}
