using Infrastructure.Configurations;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Task;

namespace Infrastructure.Data;

public class TaskManagementContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Task> Tasks { get; set; } = null!;

    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    protected TaskManagementContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TaskConfiguration());
    }
}