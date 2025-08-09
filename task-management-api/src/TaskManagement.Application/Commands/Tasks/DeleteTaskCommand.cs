using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Command for deleting a Task (Employee).
    /// </summary>
    public class DeleteTaskCommand : ICommand<Result<bool>>
    {
        public int TaskId { get; }
        public int UserId { get; }

        public DeleteTaskCommand(int taskId, int userId)
        {
            TaskId = taskId;
            UserId = userId;
        }
    }
}
