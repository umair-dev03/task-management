using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Handler for UpdateTaskStatusCommand (Manager only).
    /// </summary>
    public class UpdateTaskStatusCommandHandler : ICommandHandler<UpdateTaskStatusCommand, Result<TaskDto>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public UpdateTaskStatusCommandHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(UpdateTaskStatusCommand command, CancellationToken cancellationToken)
        {

            var task = await _taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found.");

            // Validate status
            if (!System.Enum.TryParse<Status>(command.Status, out var status))
                return Result<TaskDto>.Failure("Invalid status value.");

            task.Status = status;
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync(cancellationToken);

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
