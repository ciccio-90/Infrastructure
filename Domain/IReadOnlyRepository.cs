using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Domain
{
    public interface IReadOnlyRepository<T, TId> where T : EntityBase<TId>
    {
        T FindBy(TId id);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, int index, int count);
        IEnumerable<T> FindAll();
    }
}