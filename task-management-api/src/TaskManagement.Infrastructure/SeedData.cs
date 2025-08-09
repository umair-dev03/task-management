using System.Linq;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(TaskManagementDbContext context)
        {
            // Seed Roles
            if (!context.Roles.Any())
            {
                var employeeRole = new Role { Name = "Employee" };
                var managerRole = new Role { Name = "Manager" };
                context.Roles.AddRange(employeeRole, managerRole);
                context.SaveChanges();
            }

            // Get roles
            var employee = context.Roles.FirstOrDefault(r => r.Name == "Employee");
            var manager = context.Roles.FirstOrDefault(r => r.Name == "Manager");

            // Seed Users
            if (!context.Users.Any())
            {
                var user1 = new User
                {
                    UserName = "employee1",
                    Email = "employee1@example.com",
                    Password = "password123",
                    Roles = new List<Role> { employee }
                };
                var user2 = new User
                {
                    UserName = "manager1",
                    Email = "manager1@example.com",
                    Password = "password123",
                    Roles = new List<Role> { manager }
                };
                context.Users.AddRange(user1, user2);
                context.SaveChanges();
            }
        }
    }
}
