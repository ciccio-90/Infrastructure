namespace Infrastructure.Domain
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}