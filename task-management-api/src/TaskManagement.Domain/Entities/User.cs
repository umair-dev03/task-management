namespace TaskManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
