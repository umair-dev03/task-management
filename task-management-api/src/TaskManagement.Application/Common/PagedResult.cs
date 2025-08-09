using System.Collections.Generic;

namespace TaskManagement.Application.Common
{
    /// <summary>
    /// Generic paged result for pagination APIs.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
