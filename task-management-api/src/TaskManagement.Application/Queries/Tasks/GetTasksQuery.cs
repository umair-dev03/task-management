using TaskManagement.Application.Common;

namespace TaskManagement.Application.Queries.Tasks
{
    /// <summary>
    /// Query for getting paginated, searchable, filterable list of Tasks.
    /// </summary>
    public class GetTasksQuery : IQuery<Result<PagedResult<TaskDto>>>
    {
        public int UserId { get; }
        public string Role { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public string? SearchEmployeeName { get; }
        public string? Status { get; }

        public GetTasksQuery(int userId, string role, int pageNumber, int pageSize, string? searchEmployeeName, string? status)
        {
            UserId = userId;
            Role = role;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchEmployeeName = searchEmployeeName;
            Status = status;
        }
    }
}
