using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.DTOs;

public class TaskFilterDto
{
    public string? Title { get; set; }
    public TaskStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }

    // Pagination
    public int PageNumber { get; set; } = 1; // Default to the first page
    public int PageSize { get; set; } = 10;  // Default to 10 items per page
}
