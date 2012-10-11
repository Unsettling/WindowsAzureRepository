namespace AzurePatterns
{
    using System.Collections.Generic;

    public interface IEntityRepository : IRepository<Entity>
    {
        void Delete(Entity item);

        Entity GetEntity(params Specification<Entity>[] specifications);

        IEnumerable<Entity> GetEntities(params Specification<Entity>[] specifications);

        IEnumerable<Entity> GetEntitiesPaged(string key, int pageIndex, int pageSize);
    }
}
