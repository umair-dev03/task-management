using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Handler for CreateTaskCommand (Employee).
    /// </summary>
    public class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, Result<TaskDto>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public CreateTaskCommandHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<Result<TaskDto>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
                return Result<TaskDto>.Failure("User not found.");

            // Create new task
            var task = new TaskManagement.Domain.Entities.Task
            {
                Title = command.Title,
                Date = command.Date,
                HourWorked = command.HourWorked,
                Status = Status.Pending,
                UserId = command.UserId
            };

            await _taskRepository.AddAsync(task, cancellationToken);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var dto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Date = task.Date,
                HourWorked = task.HourWorked,
                Status = task.Status.ToString(),
                UserId = task.UserId,
                EmployeeName = user.UserName
            };

            return Result<TaskDto>.Success(dto);
        }
    }
}
