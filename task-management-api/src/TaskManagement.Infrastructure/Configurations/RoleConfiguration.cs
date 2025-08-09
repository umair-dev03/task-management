using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<TaskManagement.Domain.Entities.Role>
    {
        public void Configure(EntityTypeBuilder<TaskManagement.Domain.Entities.Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(r => r.Users)
                .WithMany(u => u.Roles);
        }
    }
}
