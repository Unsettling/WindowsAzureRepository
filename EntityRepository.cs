namespace AzurePatterns
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.WindowsAzure.StorageClient;

    // NOTE: It's an EntityRepository not a TableRepository because there
    // may be more than one kind of entity stored in a single table.
    public class EntityRepository : RepositoryBase, IEntityRepository
    {
        public EntityRepository(IUnitOfWork context) : base(context, "table")
        {
        }

        public void Save(Entity entity)
        {
            // Insert or Merge Entity aka Upsert (>= v.1.4).
            // NOTE: Upsert may not be currently supported by the emulated storage service.
            // In case we are already tracking the entity we must first detach for the Upsert to work.
            this.Context.Detach(entity);
            this.Context.AttachTo(this.Table, entity);
            this.Context.UpdateObject(entity);
        }

        public void Delete(Entity entity)
        {
            this.Context.DeleteObject(entity);
        }

        public Entity GetEntity(params Specification<Entity>[] specifications)
        {
            return this.Find(specifications).FirstOrDefault();
        }

        public IEnumerable<Entity> GetEntities(params Specification<Entity>[] specifications)
        {
            // new ByKeySpecification("partitionKey")
            return this.Find(specifications);
        }

        public IEnumerable<Entity> GetEntitiesPaged(string partitionKey, int pageIndex, int pageSize)
        {
            var results = this.Find(new ByPartitionKeySpecification("partitionKey"));

            return results.Skip(pageIndex * pageSize).Take(pageSize);
        }

        public IEnumerable<Entity> Find(params Specification<Entity>[] specifications)
        {
            IQueryable<Entity> query = this.Context.CreateQuery<Entity>(this.Table).AsTableServiceQuery();

            query = specifications.Aggregate(query, (current, spec) => current.Where(spec.Predicate));

            // TODO: does this perform Execute?
            return query.ToArray();
        }
    }
}
