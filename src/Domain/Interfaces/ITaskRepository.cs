using Domain.Specifications;
using TaskEntity = Domain.Entities.Task;

namespace Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(Guid id);
    Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetFilteredAsync(
        Specification<TaskEntity> specification,
        string? sortBy = null,
        bool sortDescending = false,
        int skip = 0,
        int take = int.MaxValue);
    Task AddAsync(TaskEntity task);
    void Update(TaskEntity task);
    void Delete(TaskEntity task);
    Task SaveChangesAsync();
}
