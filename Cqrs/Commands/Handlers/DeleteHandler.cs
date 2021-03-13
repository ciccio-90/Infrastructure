using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cqrs.Commands.Requests;
using Infrastructure.Domain;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Handlers
{
    public class DeleteHandler<T, TId> : IRequestHandler<DeleteRequest<T, TId>> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IDataService<T, TId> _dataService;

        public DeleteHandler(IDataService<T, TId> dataService)
        {
            _dataService = dataService;    
        }

        public async Task<Unit> Handle(DeleteRequest<T, TId> request, CancellationToken cancellationToken)
        {
            _dataService.Delete(request.Id);

            return await Task.FromResult<Unit>(Unit.Value);
        }
    }
}