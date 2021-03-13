using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cqrs.Commands.Requests;
using Infrastructure.Domain;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Handlers
{
    public class CreateHandler<T, TId> : IRequestHandler<CreateRequest<T, TId>, T> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IDataService<T, TId> _dataService;

        public CreateHandler(IDataService<T, TId> dataService)
        {
            _dataService = dataService;    
        }

        public async Task<T> Handle(CreateRequest<T, TId> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult<T>(_dataService.Create(request.Entity));
        }
    }
}