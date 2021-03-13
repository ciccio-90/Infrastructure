using Infrastructure.Domain;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Requests
{
    public class CreateRequest<T, TId> : IRequest<T> where T : EntityBase<TId>, IAggregateRoot
    {
        public T Entity { get; }

        public CreateRequest(T entity)
        {
            Entity = entity;
        }
    }
}