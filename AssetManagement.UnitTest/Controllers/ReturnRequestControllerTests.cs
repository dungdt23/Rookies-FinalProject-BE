using AssetManagement.Api.Controllers;
using AssetManagement.Application.Dtos.Common;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.IServices;
using AssetManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace AssetManagement.UnitTests.Controllers
{
    public class ReturnRequestsControllerTests
    {
        private readonly Mock<IReturnRequestService> _mockReturnRequestService;
        private readonly ReturnRequestsController _controller;
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";

        public ReturnRequestsControllerTests()
        {
            _mockReturnRequestService = new Mock<IReturnRequestService>();
            _controller = new ReturnRequestsController(_mockReturnRequestService.Object);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = _authorizeHeaderMock;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenReturnRequestsExist()
        {
            // Arrange
            var returnRequests = new List<ResponseReturnRequestGetAllDto>
            {
                new ResponseReturnRequestGetAllDto { Id = Guid.NewGuid(), ReturnedDate = DateTime.Today }
            };
            var totalCount = 1;
            var request = new RequestGetAllReturnRequestDto();


            _mockReturnRequestService
                .Setup(service => service.GetAllReturnRequestAsync(request, It.IsAny<Guid>()))
                .ReturnsAsync((returnRequests, totalCount));

            // Act
            var result = await _controller.GetAll(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var paginatedResult = Assert.IsType<ResponsePaginatedResultDto<ResponseReturnRequestGetAllDto>>(okResult.Value);
            Assert.Equal(returnRequests, paginatedResult.Data);
            Assert.Equal(totalCount, paginatedResult.TotalCount);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenReturnRequestCreated()
        {
            // Arrange
            var request = new RequestCreateReturnRequestDto();
            var returnRequestViewModel = new ResponseReturnRequestDto();

            _mockReturnRequestService
                .Setup(service => service.CreateReturnRequestAsync(request, It.IsAny<Guid>()))
                .ReturnsAsync(returnRequestViewModel);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(returnRequestViewModel, createdResult.Value);
        }

        [Fact]
        public async Task UpdateState_ShouldReturnNoContent_WhenCompletedSuccessfully()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var request = new RequestUpdateReturnRequestStateDto { State = TypeRequestState.Completed };

            _mockReturnRequestService
                .Setup(service => service.CompleteReturnRequestAsync(returnRequestId, It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateState(returnRequestId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateState_ShouldReturnNoContent_WhenRejectedSuccessfully()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var request = new RequestUpdateReturnRequestStateDto { State = TypeRequestState.Rejected };

            _mockReturnRequestService
                .Setup(service => service.RejectReturnRequestAsync(returnRequestId, It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateState(returnRequestId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

    }
}