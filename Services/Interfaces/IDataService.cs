using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Domain;

namespace Infrastructure.Services.Interfaces
{
    public interface IDataService<T, TId> where T : IAggregateRoot
    {
        T Get(TId id);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate, int index, int count);
        IEnumerable<T> Get();
        T Modify(T entity);
        T Create(T entity);
        void Delete(TId id);
    }
}