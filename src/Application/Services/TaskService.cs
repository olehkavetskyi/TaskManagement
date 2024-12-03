using Application.DTOs;
using Application.Interfaces;
using Domain.Specifications;
using AutoMapper;
using Domain.Interfaces;
using TaskEntity = Domain.Entities.Task;
using Serilog;

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
        try
        {
            var task = _mapper.Map<TaskEntity>(dto);
            task.UserId = userId;

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while creating a task for user {UserId}.", userId);
            throw;
        }
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, Guid userId)
    {
        try
        {
            var task = await GetTaskByIdAndUserAsync(id, userId);

            _mapper.Map(dto, task);
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while updating task with ID {TaskId} for user {UserId}.", id, userId);
            throw;
        }
    }

    public async Task<bool> DeleteTaskAsync(Guid id, Guid userId)
    {
        try
        {
            var task = await GetTaskByIdAndUserAsync(id, userId);

            _taskRepository.Delete(task);
            await _taskRepository.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while deleting task with ID {TaskId} for user {UserId}.", id, userId);
            throw;
        }
    }

    public async Task<TaskDto> GetTaskByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var task = await GetTaskByIdAndUserAsync(id, userId);
            return _mapper.Map<TaskDto>(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while fetching task with ID {TaskId} for user {UserId}.", id, userId);
            throw;
        }
    }

    public async Task<PagedResultDto<TaskDto>> GetTasksAsync(TaskFilterDto filter, Guid userId)
    {
        try
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
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while fetching tasks for user {UserId}.", userId);
            throw;
        }
    }

    // Helper to validate task ownership
    private async Task<TaskEntity> GetTaskByIdAndUserAsync(Guid taskId, Guid userId)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to this task.");
            }

            return task;
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskService>().Error(ex, "An error occurred while validating task ownership for task ID {TaskId} and user {UserId}.", taskId, userId);
            throw;
        }
    }
}
