using System;

namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for updating a Task.
    /// </summary>
    public class UpdateTaskDto
    {
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public double HourWorked { get; set; }
    }
}
