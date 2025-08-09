using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Command for updating the status of a Task (Manager only).
    /// </summary>
    public class UpdateTaskStatusCommand : ICommand<Result<TaskDto>>
    {
        public int TaskId { get; }
        public string Status { get; }
        public int UserId { get; }
        public string Role { get; }

        public UpdateTaskStatusCommand(int taskId, string status, int userId, string role)
        {
            TaskId = taskId;
            Status = status;
            UserId = userId;
            Role = role;
        }
    }
}
