using System.Linq.Expressions;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Specifications;

public class TaskSpecification : Specification<TaskEntity>
{
    private readonly string? _title;
    private readonly TaskStatus? _status;
    private readonly DateTime? _dueDate;
    private readonly Guid _userId;

    public TaskSpecification(Guid userId, string? title = null, TaskStatus? status = null, DateTime? dueDate = null)
    {
        _userId = userId;
        _title = title;
        _status = status;
        _dueDate = dueDate;
    }

    public override Expression<Func<TaskEntity, bool>> ToExpression()
    {
        return t =>
            t.UserId == _userId && 
            (string.IsNullOrEmpty(_title) || t.Title.Contains(_title)) &&
            (!_status.HasValue || t.Status == _status) &&
            (!_dueDate.HasValue || t.DueDate == _dueDate);
    }
}
