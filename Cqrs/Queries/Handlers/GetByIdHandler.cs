using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cqrs.Queries.Requests;
using Infrastructure.Domain;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Infrastructure.Cqrs.Queries.Handlers
{
    public class GetByIdHandler<T, TId> : IRequestHandler<GetByIdRequest<T, TId>, T> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IDataService<T, TId> _dataService;

        public GetByIdHandler(IDataService<T, TId> dataService)
        {
            _dataService = dataService;    
        }

        public async Task<T> Handle(GetByIdRequest<T, TId> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult<T>(_dataService.Get(request.Id));
        }
    }
}