using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Api.Controllers;
using TaskManagement.Api.Models;
using TaskManagement.Application.Common;
using Xunit;

namespace TaskManagement.Api.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Email = "employee1@example.com", Password = "password123" };
            var userDto = new TaskManagement.Application.Common.UserDto { Id = 1, UserName = "employee1", Email = "employee1@example.com", Roles = new System.Collections.Generic.List<string> { "Employee" } };
            var responseDto = new TaskManagement.Application.Common.LoginResponseDto { User = userDto, Token = "fake-jwt-token" };

            _authServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TaskManagement.Application.Common.LoginResponseDto?>(responseDto));

            // Act
            var result = await _controller.Login(loginRequest, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TaskManagement.Api.Models.LoginResponseDto>(okResult.Value);
            Assert.Equal("employee1", value.User.UserName);
            Assert.Equal("fake-jwt-token", value.Token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Email = "employee1@example.com", Password = "wrongpassword" };
            _authServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TaskManagement.Application.Common.LoginResponseDto?>(null));

            // Act
            var result = await _controller.Login(loginRequest, CancellationToken.None);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
