using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Handler for UpdateTaskCommand (Employee).
    /// </summary>
    public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, Result<TaskDto>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public UpdateTaskCommandHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            // Get task
            var task = await _taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
            if (task == null)
                return Result<TaskDto>.Failure("Task not found.");

            // Ensure the task belongs to the employee
            if (task.UserId != command.UserId)
                return Result<TaskDto>.Failure("You are not authorized to update this task.");

            // Update fields
            task.Title = command.Title;
            task.Date = command.Date;
            task.HourWorked = command.HourWorked;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            // Get user for EmployeeName
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
