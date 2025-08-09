using System;

namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for Task entity.
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public double HourWorked { get; set; }
        public string Status { get; set; } = null!;
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = null!;
    }
}
