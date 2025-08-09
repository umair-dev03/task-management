using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<TaskManagement.Domain.Entities.User>
    {
        public void Configure(EntityTypeBuilder<TaskManagement.Domain.Entities.User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            builder.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
