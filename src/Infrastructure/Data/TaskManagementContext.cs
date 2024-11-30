using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class TaskManagementContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Task> Tasks { get; set; }

    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    protected TaskManagementContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}