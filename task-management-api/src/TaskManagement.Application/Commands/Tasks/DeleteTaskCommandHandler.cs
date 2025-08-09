using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Handler for DeleteTaskCommand (Employee).
    /// </summary>
    public class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand, Result<bool>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;

        public DeleteTaskCommandHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async System.Threading.Tasks.Task<Result<bool>> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            // Get task
            var task = await _taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
            if (task == null)
                return Result<bool>.Failure("Task not found.");

            // Ensure the task belongs to the employee
            if (task.UserId != command.UserId)
                return Result<bool>.Failure("You are not authorized to delete this task.");

            _taskRepository.Delete(task);

            await _taskRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
