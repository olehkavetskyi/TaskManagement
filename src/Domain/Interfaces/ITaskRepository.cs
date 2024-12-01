using TaskEntity = Domain.Entities.Task;

namespace Domain.Interfaces;

public interface ITaskRepository
{
    IQueryable<TaskEntity> GetAll();
    Task<TaskEntity> GetByIdAsync(int taskId);
    Task AddAsync(TaskEntity task);
    void Update(TaskEntity task);
    void Delete(TaskEntity task);
    Task SaveChangesAsync();
}
