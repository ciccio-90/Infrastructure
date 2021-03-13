using Infrastructure.Domain;
using MediatR;

namespace Infrastructure.Cqrs.Queries.Requests
{
    public class GetByIdRequest<T, TId> : IRequest<T> where T : EntityBase<TId>, IAggregateRoot
    {
        public TId Id { get; }

        public GetByIdRequest(TId id)
        {
            Id = id;
        }
    }
}