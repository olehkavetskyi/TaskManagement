using Application.DTOs;

namespace Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId);
    Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, Guid userId);
    Task<bool> DeleteTaskAsync(Guid id, Guid userId);
    Task<TaskDto> GetTaskByIdAsync(Guid id, Guid userId);
    Task<PagedResultDto<TaskDto>> GetTasksAsync(TaskFilterDto filter, Guid userId);
}
