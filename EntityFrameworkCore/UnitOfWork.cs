using Infrastructure.Domain;
using Infrastructure.UnitOfWork;

namespace Infrastructure.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dataContext.Update(entity);
        }

        public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dataContext.Add(entity);
        }

        public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dataContext.Remove(entity);
        }

        public void Commit()
        {
            _dataContext.SaveChanges();
        }
    }
}