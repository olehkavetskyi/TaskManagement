using Infrastructure.Data;
using Domain.Interfaces;
using TaskEntity = Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Domain.Specifications;

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
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetFilteredAsync(
        Specification<TaskEntity> specification,
        string? sortBy = null,
        bool sortDescending = false,
        int skip = 0,
        int take = int.MaxValue)
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

    public async Task AddAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public void Update(TaskEntity task)
    {
        _context.Tasks.Update(task);
    }

    public void Delete(TaskEntity task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
