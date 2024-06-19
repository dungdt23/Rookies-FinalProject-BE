using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.UnitTest.Controllers.Assignments
{
    [TestFixture]
    public class AssignmentControllerPutTest
    {
        private AssignmentsController _controller;
        private Mock<IAssignmentService> _assignmentServiceMock;
        private Mock<RequestAssignmentDto> _requestDtoMock;
        private Mock<ResponseAssignmentDto> _assignmentDtoMock;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assignmentServiceMock = new Mock<IAssignmentService>();
            _controller = new AssignmentsController(_assignmentServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _requestDtoMock = new Mock<RequestAssignmentDto>();
            _assignmentDtoMock = new Mock<ResponseAssignmentDto>();
        }

        [Test]
        public async Task Put_ShouldReturnNotFound_WhenCantFoundAssignmentWithId()
        {
            //Arrange
            var id = Guid.NewGuid();

            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
                Data = id
            };

            _assignmentServiceMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestAssignmentDto>())).ReturnsAsync(apiResponse);

            //Act
            var result = await _controller.Put(id, _requestDtoMock.Object);

            //Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        public async Task Put_ShouldReturnError_WhenUpdateAssignmentFail()
        {
            //Arrange
            var id = Guid.NewGuid();

            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = AssignmentApiResponseMessageConstant.AssignmentUpdateFail,
                Data = _assignmentDtoMock.Object
            };

            _assignmentServiceMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestAssignmentDto>())).ReturnsAsync(apiResponse);

            //Act
            var result = await _controller.Put(id, _requestDtoMock.Object);

            //Assert
            var errorResult = result as ObjectResult;
            errorResult.Should().NotBeNull();
            errorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            errorResult.Value.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        public async Task Put_ShouldReturnSuccess_WhenUpdateAssignmentSuccess()
        {
            //Arrange
            var id = Guid.NewGuid();

            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = AssignmentApiResponseMessageConstant.AssignmentUpdateSuccess,
                Data = _assignmentDtoMock.Object
            };

            _assignmentServiceMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestAssignmentDto>())).ReturnsAsync(apiResponse);

            //Act
            var result = await _controller.Put(id, _requestDtoMock.Object);

            //Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(apiResponse);
        }


    }
}
