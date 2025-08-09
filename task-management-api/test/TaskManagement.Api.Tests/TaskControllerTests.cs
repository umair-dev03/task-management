using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Api.Controllers;
using TaskManagement.Application.Common;
using TaskManagement.Application.Commands.Tasks;
using TaskManagement.Application.Queries.Tasks;
using Xunit;

namespace TaskManagement.Api.Tests
{
    public class TaskControllerTests
    {
        private readonly Mock<IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>> _getTasksHandlerMock;
        private readonly Mock<ICommandHandler<CreateTaskCommand, Result<TaskDto>>> _createTaskHandlerMock;
        private readonly Mock<IQueryHandler<GetTaskByIdQuery, Result<TaskDto>>> _getTaskByIdHandlerMock;
        private readonly Mock<ICommandHandler<UpdateTaskCommand, Result<TaskDto>>> _updateTaskHandlerMock;
        private readonly Mock<ICommandHandler<UpdateTaskStatusCommand, Result<TaskDto>>> _updateTaskStatusHandlerMock;
        private readonly Mock<ICommandHandler<DeleteTaskCommand, Result<bool>>> _deleteTaskHandlerMock;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _getTasksHandlerMock = new Mock<IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>>();
            _createTaskHandlerMock = new Mock<ICommandHandler<CreateTaskCommand, Result<TaskDto>>>();
            _getTaskByIdHandlerMock = new Mock<IQueryHandler<GetTaskByIdQuery, Result<TaskDto>>>();
            _updateTaskHandlerMock = new Mock<ICommandHandler<UpdateTaskCommand, Result<TaskDto>>>();
            _updateTaskStatusHandlerMock = new Mock<ICommandHandler<UpdateTaskStatusCommand, Result<TaskDto>>>();
            _deleteTaskHandlerMock = new Mock<ICommandHandler<DeleteTaskCommand, Result<bool>>>();

            _controller = new TaskController(
                _getTasksHandlerMock.Object,
                _createTaskHandlerMock.Object,
                _getTaskByIdHandlerMock.Object,
                _updateTaskHandlerMock.Object,
                _updateTaskStatusHandlerMock.Object,
                _deleteTaskHandlerMock.Object
            );

            // Mock user claims for controller context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Employee")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenSuccess()
        {
            var pagedResult = new PagedResult<TaskDto>
            {
                Items = new List<TaskDto> { new TaskDto { Id = 1, Title = "Test Task" } },
                TotalCount = 1
            };
            _getTasksHandlerMock.Setup(h => h.Handle(It.IsAny<GetTasksQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PagedResult<TaskDto>>.Success(pagedResult));

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<PagedResult<TaskDto>>(okResult.Value);
            Assert.Single(value.Items);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var taskDto = new TaskDto { Id = 1, Title = "Test Task" };
            _getTaskByIdHandlerMock.Setup(h => h.Handle(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TaskDto>.Success(taskDto));

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(1, value.Id);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenSuccess()
        {
            var createDto = new CreateTaskDto { Title = "New Task", Date = System.DateTime.Now, HourWorked = 2 };
            var taskDto = new TaskDto { Id = 1, Title = "New Task" };
            _createTaskHandlerMock.Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TaskDto>.Success(taskDto));

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsType<TaskDto>(createdResult.Value);
            Assert.Equal("New Task", value.Title);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var updateDto = new UpdateTaskDto { Title = "Updated Task", Date = System.DateTime.Now, HourWorked = 3 };
            var taskDto = new TaskDto { Id = 1, Title = "Updated Task" };
            _updateTaskHandlerMock.Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TaskDto>.Success(taskDto));

            var result = await _controller.Update(1, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal("Updated Task", value.Title);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOk_WhenSuccess()
        {
            var statusDto = new UpdateTaskStatusDto { Status = "Completed" };
            var taskDto = new TaskDto { Id = 1, Title = "Task", Status = "Completed" };
            _updateTaskStatusHandlerMock.Setup(h => h.Handle(It.IsAny<UpdateTaskStatusCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TaskDto>.Success(taskDto));

            var result = await _controller.UpdateStatus(1, statusDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal("Completed", value.Status);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _deleteTaskHandlerMock.Setup(h => h.Handle(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
