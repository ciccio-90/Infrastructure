using Infrastructure.Domain;

namespace Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void Commit();
    }
}