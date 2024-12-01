using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Task = Domain.Entities.Task;

namespace Infrastructure.Data.Identity;

public class AppUser : IdentityUser<Guid>, IUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property for related tasks
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}