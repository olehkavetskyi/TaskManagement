using System.Linq.Expressions;

namespace Domain.Specifications;

/// <summary>
/// Base class for defining query specifications.
/// Allows encapsulating filtering logic for entities.
/// </summary>
/// <typeparam name="T">The type of entity being filtered.</typeparam>
public abstract class Specification<T>
{
    /// <summary>
    /// Defines the filtering logic as an expression.
    /// Implement this in derived classes to specify the query conditions.
    /// </summary>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Compiles the filtering logic into a predicate function.
    /// Useful for in-memory filtering or direct evaluations.
    /// </summary>
    /// <returns>A function to test if an entity matches the criteria.</returns>
    public Func<T, bool> ToPredicate()
    {
        return ToExpression().Compile();
    }
}