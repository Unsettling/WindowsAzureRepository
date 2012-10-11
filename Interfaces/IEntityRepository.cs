namespace AzurePatterns.Interfaces
{
    using System.Collections.Generic;

    using AzurePatterns.Entities;

    public interface IEntityRepository : IRepository<Entity>
    {
        void Delete(Entity item);

        Entity GetEntity(params Specification<Entity>[] specifications);

        IEnumerable<Entity> GetEntities(params Specification<Entity>[] specifications);

        IEnumerable<Entity> GetEntitiesPaged(string key, int pageIndex, int pageSize);
    }
}
