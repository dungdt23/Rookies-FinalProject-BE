using AssetManagement.Application.Dtos.Common;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Exceptions.Assignment;
using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.Exceptions.ReturnRequest;
using AssetManagement.Application.IServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Enums;
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
        public async Task<IActionResult> GetAll([FromQuery] RequestGetAllReturnRequestDto request)
        {
            var userIdClaim = HttpContext.GetClaim("id");
            Guid userId = Guid.Parse(userIdClaim);
            var (returnRequests, totalCount) = await _returnRequestService.GetAllReturnRequestAsync(request, userId);
            ResponsePaginatedResultDto<ResponseReturnRequestGetAllDto> paginateResult = new ResponsePaginatedResultDto<ResponseReturnRequestGetAllDto>
            {
                Data = returnRequests,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PerPage
            };
            return Ok(paginateResult);
        }

        [HttpPost]
        [Authorize(Roles = $"{TypeNameConstants.TypeAdmin}, {TypeNameConstants.TypeStaff}")]
        public async Task<IActionResult> Create([FromBody] RequestCreateReturnRequestDto request)
        {
            var userIdClaim = HttpContext.GetClaim("id");
            Guid userId = Guid.Parse(userIdClaim);
            try
            {
                var returnRequest = await _returnRequestService.CreateReturnRequestAsync(request, userId);
                return Created("", returnRequest);
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

        [HttpPost("{returnRequestId}/state")]
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> UpdateState(Guid returnRequestId, [FromBody] RequestUpdateReturnRequestStateDto request)
        {
            if (request.State == TypeRequestState.WaitingForReturning)
                return BadRequest();

            var userIdClaim = HttpContext.GetClaim("id");
            Guid userId = Guid.Parse(userIdClaim);
            try
            {
                if (request.State == TypeRequestState.Completed)
                {
                    await _returnRequestService.CompleteReturnRequestAsync(returnRequestId, userId);
                    return NoContent();
                }

                await _returnRequestService.RejectReturnRequestAsync(returnRequestId, userId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WrongLocationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ReturnRequestNotWaitingException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
