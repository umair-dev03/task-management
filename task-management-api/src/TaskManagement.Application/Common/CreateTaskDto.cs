using System;

namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for creating a Task.
    /// </summary>
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public double HourWorked { get; set; }
    }
}
