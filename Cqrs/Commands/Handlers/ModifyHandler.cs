using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Cqrs.Commands.Requests;
using Infrastructure.Domain;
using Infrastructure.Services.Interfaces;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Handlers
{
    public class ModifyHandler<T, TId> : IRequestHandler<ModifyRequest<T, TId>, T> where T : EntityBase<TId>, IAggregateRoot
    {
        private readonly IDataService<T, TId> _dataService;

        public ModifyHandler(IDataService<T, TId> dataService)
        {
            _dataService = dataService;    
        }

        public async Task<T> Handle(ModifyRequest<T, TId> request, CancellationToken cancellationToken)
        {
            return await Task.FromResult<T>(_dataService.Modify(request.Entity));
        }
    }
}