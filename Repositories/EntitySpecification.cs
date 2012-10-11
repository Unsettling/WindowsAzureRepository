namespace AzurePatterns.Repositories
{
    using System;
    using System.Linq.Expressions;

    using AzurePatterns.Entities;

    // NOTE: Although these deserve a file to themselves I would move them into
    // the Repository implementation where they are used for convienience.
    public class ByPartitionKeySpecification : Specification<Entity>
    {
        private readonly string key;

        public ByPartitionKeySpecification(string key)
        {
            this.key = key;
        }

        public override Expression<Func<Entity, bool>> Predicate
        {
            get
            {
                return p => p.PartitionKey == this.key;
            }
        }
    }

    public class ByRowKeySpecification : Specification<Entity>
    {
        private readonly string key;

        public ByRowKeySpecification(string key)
        {
            this.key = key;
        }

        public override Expression<Func<Entity, bool>> Predicate
        {
            get
            {
                return p => p.RowKey == this.key;
            }
        }
    }
}
