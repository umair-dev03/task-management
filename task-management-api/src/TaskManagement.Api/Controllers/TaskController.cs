using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Commands.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.Queries.Tasks;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>> _getTasksHandler;
        private readonly ICommandHandler<CreateTaskCommand, Result<TaskDto>> _createTaskHandler;
        private readonly IQueryHandler<GetTaskByIdQuery, Result<TaskDto>> _getTaskByIdHandler;
        private readonly ICommandHandler<UpdateTaskCommand, Result<TaskDto>> _updateTaskHandler;
        private readonly ICommandHandler<UpdateTaskStatusCommand, Result<TaskDto>> _updateTaskStatusHandler;
        private readonly ICommandHandler<DeleteTaskCommand, Result<bool>> _deleteTaskHandler;

        public TaskController(
            IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>> getTasksHandler,
            ICommandHandler<CreateTaskCommand, Result<TaskDto>> createTaskHandler,
            IQueryHandler<GetTaskByIdQuery, Result<TaskDto>> getTaskByIdHandler,
            ICommandHandler<UpdateTaskCommand, Result<TaskDto>> updateTaskHandler,
            ICommandHandler<UpdateTaskStatusCommand, Result<TaskDto>> updateTaskStatusHandler,
            ICommandHandler<DeleteTaskCommand, Result<bool>> deleteTaskHandler)
        {
            _getTasksHandler = getTasksHandler;
            _createTaskHandler = createTaskHandler;
            _getTaskByIdHandler = getTaskByIdHandler;
            _updateTaskHandler = updateTaskHandler;
            _updateTaskStatusHandler = updateTaskStatusHandler;
            _deleteTaskHandler = deleteTaskHandler;
        }

        // GET: api/task
        [HttpGet]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchEmployeeName = null, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = new GetTasksQuery(userId, role, page, pageSize, searchEmployeeName, status);
            var result = await _getTasksHandler.Handle(query, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        // GET: api/task/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = new GetTaskByIdQuery(id, userId, role);
            var result = await _getTaskByIdHandler.Handle(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Value);
        }

        // POST: api/task
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new CreateTaskCommand(userId, dto.Title, dto.Date, dto.HourWorked);
            var result = await _createTaskHandler.Handle(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/task/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var command = new UpdateTaskCommand(id, userId, dto.Title, dto.Date, dto.HourWorked);
            var result = await _updateTaskHandler.Handle(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        // PATCH: api/task/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var command = new UpdateTaskStatusCommand(id, dto.Status, userId, role);
            var result = await _updateTaskStatusHandler.Handle(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        // DELETE: api/task/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new DeleteTaskCommand(id, userId);
            var result = await _deleteTaskHandler.Handle(command, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return NoContent();
        }
    }
}
