using Infrastructure.Data;
using Domain.Interfaces;
using TaskEntity = Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Domain.Specifications;
using Serilog;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementContext _context;

    public TaskRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task<TaskEntity?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Tasks.FindAsync(id);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while fetching task with ID {TaskId}.", id);
            throw;
        }
    }

    public async Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetFilteredAsync(
        Specification<TaskEntity> specification,
        string? sortBy = null,
        bool sortDescending = false,
        int skip = 0,
        int take = int.MaxValue)
    {
        try
        {
            IQueryable<TaskEntity> query = _context.Tasks.Where(specification.ToExpression());

            // Get the total count before applying pagination
            int totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            // Apply pagination
            var tasks = await query.Skip(skip).Take(take).ToListAsync();

            return (tasks, totalCount);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while fetching filtered tasks.");
            throw;
        }
    }

    public async Task AddAsync(TaskEntity task)
    {
        try
        {
            await _context.Tasks.AddAsync(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while adding a new task.");
            throw;
        }
    }

    public void Update(TaskEntity task)
    {
        try
        {
            _context.Tasks.Update(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while updating task with ID {TaskId}.", task.Id);
            throw;
        }
    }

    public void Delete(TaskEntity task)
    {
        try
        {
            _context.Tasks.Remove(task);
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while deleting task with ID {TaskId}.", task.Id);
            throw;
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.ForContext<TaskRepository>().Error(ex, "An error occurred while saving changes to the database.");
            throw;
        }
    }
}
