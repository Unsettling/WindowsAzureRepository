namespace AzurePatterns
{
    using System;
    using System.Linq.Expressions;

    public abstract class Specification<TEntity>
    {
        public abstract Expression<Func<TEntity, bool>> Predicate { get; }
    }
}
