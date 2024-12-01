using Infrastructure.Data;
using Domain.Interfaces;
using TaskEntity = Domain.Entities.Task;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementContext _context;

    public TaskRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public IQueryable<TaskEntity> GetAll()
    {
        return _context.Tasks.AsQueryable();
    }

    public async Task<TaskEntity> GetByIdAsync(int taskId)
    {
        return await _context.Tasks.FindAsync(taskId);
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
