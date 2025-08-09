using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskManagement.Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<TaskManagement.Domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (Status)Enum.Parse(typeof(Status), v))
                .IsRequired();

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.HourWorked)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId);
        }
    }
}
