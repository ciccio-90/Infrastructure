using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Domain;
using MediatR;

namespace Infrastructure.Cqrs.Queries.Requests
{
    public class GetRequest<T, TId> : IRequest<IEnumerable<T>> where T : EntityBase<TId>, IAggregateRoot
    {
        public Expression<Func<T, bool>> Predicate { get; }
        public int? Index { get; }
        public int? Count { get; }

        public GetRequest(Expression<Func<T, bool>> predicate, int? index, int? count)
        {
            Predicate = predicate;
            Index = index;
            Count = count;
        }
    }
}