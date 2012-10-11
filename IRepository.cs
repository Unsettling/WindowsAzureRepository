namespace AzurePatterns
{
    using System.Collections.Generic;

    using Microsoft.WindowsAzure.StorageClient;

    public interface IRepository<TEntity> where TEntity : TableServiceEntity
    {
        IEnumerable<TEntity> Find(params Specification<TEntity>[] specifications);

        void Save(TEntity item);
    }
}