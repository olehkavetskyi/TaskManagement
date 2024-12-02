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

    public async Task<IEnumerable<TaskEntity>> GetFilteredAsync(
        Specification<TaskEntity> specification,
        string? sortBy = null,
        bool sortDescending = false)
    {
        IQueryable<TaskEntity> query = _context.Tasks.Where(specification.ToExpression());

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                : query.OrderBy(e => EF.Property<object>(e, sortBy));
        }

        return await query.ToListAsync();
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
