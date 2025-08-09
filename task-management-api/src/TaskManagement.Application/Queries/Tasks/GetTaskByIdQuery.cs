using TaskManagement.Application.Common;

namespace TaskManagement.Application.Queries.Tasks
{
    /// <summary>
    /// Query for getting a Task by Id (Employee: own, Manager: any).
    /// </summary>
    public class GetTaskByIdQuery : IQuery<Result<TaskDto>>
    {
        public int TaskId { get; }
        public int UserId { get; }
        public string Role { get; }

        public GetTaskByIdQuery(int taskId, int userId, string role)
        {
            TaskId = taskId;
            UserId = userId;
            Role = role;
        }
    }
}
