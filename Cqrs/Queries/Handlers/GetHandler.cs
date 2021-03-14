using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cqrs.Queries.Requests;
using Infrastructure.Domain;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Infrastructure.Cqrs.Queries.Handlers
{
    public class GetHandler<T, TId> : IRequestHandler<GetRequest<T, TId>, IEnumerable<T>> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IDataService<T, TId> _dataService;

        public GetHandler(IDataService<T, TId> dataService)
        {
            _dataService = dataService;    
        }

        public async Task<IEnumerable<T>> Handle(GetRequest<T, TId> request, CancellationToken cancellationToken)
        {
            if (request?.Predicate != null)
            {
                if (request?.Index != null && request?.Count != null)
                {
                    return await Task.FromResult<IEnumerable<T>>(_dataService.Get(request.Predicate, request.Index.Value, request.Count.Value));
                }
                else
                {
                    return await Task.FromResult<IEnumerable<T>>(_dataService.Get(request.Predicate));
                }
            }
            else
            {
                if (request?.Index != null && request?.Count != null)
                {
                    return await Task.FromResult<IEnumerable<T>>(_dataService.Get(_ => true, request.Index.Value, request.Count.Value));
                }
                else
                {
                    return await Task.FromResult<IEnumerable<T>>(_dataService.Get());
                }
            }
        }
    }
}
