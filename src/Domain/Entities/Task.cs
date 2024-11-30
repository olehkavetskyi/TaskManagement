using Domain.Enums;
using Domain.Interfaces;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Entities;

public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key for user
    public string UserId { get; set; } = string.Empty;
    // Navigation property using IUser for AppUser
    public IUser User { get; set; } = null!;

}