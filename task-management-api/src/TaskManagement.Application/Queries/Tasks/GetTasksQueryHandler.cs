using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Queries.Tasks
{
    /// <summary>
    /// Handler for GetTasksQuery (Employee: own, Manager: all, with search/filter/pagination).
    /// </summary>
    public class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;
        private readonly IRepository<User> _userRepository;

        public GetTasksQueryHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository, IRepository<User> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async System.Threading.Tasks.Task<Result<PagedResult<TaskDto>>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
        {
            // Get all users for EmployeeName search
            var users = (await _userRepository.GetAllAsync(cancellationToken)).ToList();

            // Build base query
            var tasks = (await _taskRepository.GetAllAsync(cancellationToken)).AsQueryable();

            // Role-based filtering
            if (query.Role == "Employee")
                tasks = tasks.Where(t => t.UserId == query.UserId);

            // Filter by Status
            if (!string.IsNullOrWhiteSpace(query.Status))
                tasks = tasks.Where(t => t.Status.ToString() == query.Status);

            // Search by Employee name
            if (!string.IsNullOrWhiteSpace(query.SearchEmployeeName))
            {
                var userIds = users
                    .Where(u => u.UserName.Contains(query.SearchEmployeeName))
                    .Select(u => u.Id)
                    .ToList();
                tasks = tasks.Where(t => userIds.Contains(t.UserId));
            }

            // Total count before pagination
            var totalCount = tasks.Count();

            // Pagination
            var pagedTasks = tasks
                .OrderByDescending(t => t.Date)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            // Map to DTOs
            var items = pagedTasks.Select(t =>
            {
                var user = users.FirstOrDefault(u => u.Id == t.UserId);
                return new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Date = t.Date,
                    HourWorked = t.HourWorked,
                    Status = t.Status.ToString(),
                    UserId = t.UserId,
                    EmployeeName = user?.UserName ?? ""
                };
            }).ToList();

            var result = new PagedResult<TaskDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PagedResult<TaskDto>>.Success(result);
        }
    }
}
