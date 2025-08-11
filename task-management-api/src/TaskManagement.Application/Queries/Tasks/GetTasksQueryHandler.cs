using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Queries.Tasks
{
    /// <summary>
    /// Handler for GetTasksQuery (Employee: own, Manager: all, with search/filter/pagination).
    /// </summary>
    public class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
    {
        private readonly IRepository<TaskManagement.Domain.Entities.Task> _taskRepository;

        public GetTasksQueryHandler(IRepository<TaskManagement.Domain.Entities.Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async System.Threading.Tasks.Task<Result<PagedResult<TaskDto>>> Handle(GetTasksQuery query, CancellationToken cancellationToken)
        {
            IQueryable<TaskManagement.Domain.Entities.Task> tasks = _taskRepository.Query().AsNoTracking();

            // Role-based filtering
            if (query.Role == "Employee")
                tasks = tasks.Where(t => t.UserId == query.UserId);

            // Filter by Status (parse once to enum for proper translation)
            if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<Status>(query.Status, true, out var statusEnum))
                tasks = tasks.Where(t => t.Status == statusEnum);

            // Search by Employee name (translated to SQL via navigation)
            if (!string.IsNullOrWhiteSpace(query.SearchEmployeeName))
            {
                var pattern = $"%{query.SearchEmployeeName}%";
                tasks = tasks.Where(t => t.User != null && EF.Functions.Like(t.User.UserName, pattern));
            }

            // Total count before pagination
            var totalCount = await tasks.CountAsync(cancellationToken);

            // Pagination + Projection (single SQL roundtrip)
            var items = await tasks
                .OrderByDescending(t => t.Date)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Date = t.Date,
                    HourWorked = t.HourWorked,
                    Status = t.Status.ToString(),
                    UserId = t.UserId,
                    EmployeeName = t.User != null ? t.User.UserName : ""
                })
                .ToListAsync(cancellationToken);

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
