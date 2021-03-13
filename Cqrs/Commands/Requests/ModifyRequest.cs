using Infrastructure.Domain;
using MediatR;

namespace Infrastructure.Cqrs.Commands.Requests
{
    public class ModifyRequest<T, TId> : IRequest<T> where T : EntityBase<TId>, IAggregateRoot
    {
        public T Entity { get; }

        public ModifyRequest(T entity)
        {
            Entity = entity;
        }
    }
}