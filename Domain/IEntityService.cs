using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Domain
{
    public interface IEntityService<T, TId> where T : EntityBase<TId>
    {
        T Get(TId id);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate, int index, int count);
        IEnumerable<T> Get();
        T Create(T entity);
        T Modify(T entity);
        void Delete(TId id);
    }
}