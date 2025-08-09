using TaskManagement.Application.Common;

namespace TaskManagement.Application.Commands.Tasks
{
    /// <summary>
    /// Command for creating a new Task (Employee).
    /// </summary>
    public class CreateTaskCommand : ICommand<Result<TaskDto>>
    {
        public int UserId { get; }
        public string Title { get; }
        public DateTime Date { get; }
        public double HourWorked { get; }

        public CreateTaskCommand(int userId, string title, DateTime date, double hourWorked)
        {
            UserId = userId;
            Title = title;
            Date = date;
            HourWorked = hourWorked;
        }
    }
}
