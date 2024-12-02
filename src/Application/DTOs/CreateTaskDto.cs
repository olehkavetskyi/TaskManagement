using TaskStatus = Domain.Enums.TaskStatus;
using TaskPriority = Domain.Enums.TaskPriority;

namespace Application.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}