using Application.DTOs;
using Application.Interfaces;
using Domain.Specifications;
using AutoMapper;
using Domain.Interfaces;
using TaskEntity = Domain.Entities.Task;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId)
    {
        var task = _mapper.Map<TaskEntity>(dto);
        task.UserId = userId;

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, Guid userId)
    {
        var task = await GetTaskByIdAndUserAsync(id, userId);

        _mapper.Map(dto, task);
        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<bool> DeleteTaskAsync(Guid id, Guid userId)
    {
        var task = await GetTaskByIdAndUserAsync(id, userId);

        _taskRepository.Delete(task);
        await _taskRepository.SaveChangesAsync();

        return true;
    }

    public async Task<TaskDto> GetTaskByIdAsync(Guid id, Guid userId)
    {
        var task = await GetTaskByIdAndUserAsync(id, userId);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<PagedResultDto<TaskDto>> GetTasksAsync(TaskFilterDto filter, Guid userId)
    {
        var specification = new TaskSpecification(userId, filter.Title, filter.Status, filter.DueDate);

        var skip = (filter.PageNumber - 1) * filter.PageSize;
        var (tasks, totalCount) = await _taskRepository.GetFilteredAsync(
            specification,
            filter.SortBy,
            filter.SortDescending,
            skip,
            filter.PageSize
        );

        return new PagedResultDto<TaskDto>
        {
            Items = _mapper.Map<IEnumerable<TaskDto>>(tasks),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }


    // Helper to validate task ownership
    private async Task<TaskEntity> GetTaskByIdAndUserAsync(Guid taskId, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

        return task;
    }
}

