using TaskStatus = Domain.Enums.TaskStatus;
using TaskPriority = Domain.Enums.TaskPriority;


namespace Application.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? TaskPriority { get; set; }
}
