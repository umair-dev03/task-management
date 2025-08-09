using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public double HourWorked { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
