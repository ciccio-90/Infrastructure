using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Domain;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EntityFrameworkCore
{
    public class Repository<T, EntityKey> : IRepository<T, EntityKey>, IUnitOfWorkRepository where T : EntityBase<EntityKey>, IAggregateRoot
    {
        protected readonly IUnitOfWork _uow;
        protected readonly DataContext _dataContext;

        public Repository(IUnitOfWork uow, DataContext dataContext)
        {
            _uow = uow;
            _dataContext = dataContext;
        }

        public void Add(T entity)
        {
            PersistCreationOf(entity);
        }

        public void Remove(T entity)
        {
            PersistDeletionOf(entity);
        }

        public void Save(T entity)
        {
            PersistUpdateOf(entity);
        }

        public T FindBy(EntityKey id)
        {
            return AppendCriteria(_dataContext.Set<T>()).OrderBy(e => e.Id).FirstOrDefault(e => e.Id.Equals(id));
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return AppendCriteria(_dataContext.Set<T>()).Where(predicate);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, int index, int count)
        {
            return AppendCriteria(_dataContext.Set<T>()).Where(predicate).OrderBy(e => e.Id).Skip(index).Take(count);
        }

        public IEnumerable<T> FindAll()
        {
            return AppendCriteria(_dataContext.Set<T>());
        }

        public IEnumerable<T> FindAll(int index, int count)
        {
            return AppendCriteria(_dataContext.Set<T>()).OrderBy(e => e.Id).Skip(index).Take(count);
        }

        public virtual IQueryable<T> AppendCriteria(IQueryable<T> criteria)
        {
            return criteria;
        }

        public void PersistCreationOf(IAggregateRoot entity)
        {
            _uow.RegisterNew(entity, this);
        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            _uow.RegisterAmended(entity, this);
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            _uow.RegisterRemoved(entity, this);
        }
    }
}