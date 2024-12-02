using Domain.Specifications;
using TaskEntity = Domain.Entities.Task;

namespace Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskEntity>> GetFilteredAsync(
        Specification<TaskEntity> specification,
        string? sortBy = null,
        bool sortDescending = false);
    Task AddAsync(TaskEntity task);
    void Update(TaskEntity task);
    void Delete(TaskEntity task);
    Task SaveChangesAsync();
}
