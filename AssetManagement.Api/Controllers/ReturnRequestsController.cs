using AssetManagement.Application.Dtos.Common;
using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.Exceptions.Assignment;
using AssetManagement.Application.Exceptions.Common;
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
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllReturnRequest request)
        {
            var userIdClaim = HttpContext.GetClaim("id");
            Guid userId = Guid.Parse(userIdClaim);
            try
            {
                var (returnRequests, totalCount) = await _returnRequestService.GetAllReturnRequestAsync(request, userId);
                PaginatedResult<ReturnRequestGetAllViewModel> paginateResult = new PaginatedResult<ReturnRequestGetAllViewModel>
                {
                    Data = returnRequests,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PerPage
                };
                return Ok(paginateResult);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{TypeNameConstants.TypeAdmin}, {TypeNameConstants.TypeStaff}")]
        public async Task<IActionResult> Create([FromBody] CreateReturnRequestRequest request)
        {
            var userIdClaim = HttpContext.GetClaim("id");
            Guid userId = Guid.Parse(userIdClaim);
            try
            {
                var returnRequest = await _returnRequestService.CreateReturnRequestAsync(request, userId);
                return Ok(returnRequest);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WrongLocationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AssignmentNotAcceptedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAssignmentAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ActiveReturnRequestAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
