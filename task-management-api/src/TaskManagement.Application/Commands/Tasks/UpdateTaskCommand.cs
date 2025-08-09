using System;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Command for updating a Task (Employee).
    /// </summary>
    public class UpdateTaskCommand : ICommand<Result<TaskDto>>
    {
        public int TaskId { get; }
        public int UserId { get; }
        public string Title { get; }
        public DateTime Date { get; }
        public double HourWorked { get; }

        public UpdateTaskCommand(int taskId, int userId, string title, DateTime date, double hourWorked)
        {
            TaskId = taskId;
            UserId = userId;
            Title = title;
            Date = date;
            HourWorked = hourWorked;
        }
    }
}
