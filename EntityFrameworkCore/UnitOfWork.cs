using Infrastructure.Domain;

namespace Infrastructure.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Commit()
        {
            _dataContext.SaveChanges();
        }
    }
}