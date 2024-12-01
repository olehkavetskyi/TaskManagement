using Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Task;

namespace Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(100);

        builder.Ignore(t => t.User);

        builder.Property(t => t.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");  

        builder.Property(t => t.UpdatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");  

        builder.HasOne<AppUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
