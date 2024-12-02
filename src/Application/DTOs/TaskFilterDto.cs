using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.DTOs;

public class TaskFilterDto
{
    public string? Title { get; set; }
    public TaskStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
