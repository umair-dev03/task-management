namespace TaskManagement.Application.Common
{
    /// <summary>
    /// DTO for updating the status of a Task (Manager only).
    /// </summary>
    public class UpdateTaskStatusDto
    {
        public string Status { get; set; } = null!;
    }
}
