using Infrastructure.Domain;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Requests
{
    public class DeleteRequest<T, TId> : IRequest where T : EntityBase<TId>, IAggregateRoot
    {
        public TId Id { get; }

        public DeleteRequest(TId id)
        {
            Id = id;
        }
    }
}