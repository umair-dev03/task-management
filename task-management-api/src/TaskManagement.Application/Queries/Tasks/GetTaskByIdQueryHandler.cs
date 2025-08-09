using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Queries.Tasks
{
    /// <summary>
    /// Handler for GetTaskByIdQuery (Employee: own, Manager: any).
    /// </summary>
    public class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, Result<TaskDto>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public GetTaskByIdQueryHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(query.TaskId, cancellationToken);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found.");

            // Role-based access
            if (query.Role == "Employee" && task.UserId != query.UserId)
                return Result<TaskDto>.Failure("You are not authorized to view this task.");

            var user = await _userRepository.GetByIdAsync(task.UserId, cancellationToken);

            var dto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Date = task.Date,
                HourWorked = task.HourWorked,
                Status = task.Status.ToString(),
                UserId = task.UserId,
                EmployeeName = user?.UserName ?? ""
            };

            return Result<TaskDto>.Success(dto);
        }
    }
}
