namespace AzurePatterns
{
    using AzurePatterns.Repositories;
    using AzurePatterns.TableStorage;

    public class EntityRepositoryTests
    {
        // NOTE: Windows Azure changes a lot with each release. 
        // This code was written against version 1.7 (June 2012).
        public void ShouldAlterEntity()
        {
            // NOTE: expect the context and the repository to be managed by your IoC container.
            var context = new TableStorageContext();
            var entityRepository = new EntityRepository(context);
                    
            var partitionKey = new ByPartitionKeySpecification("partitionKey");
            var rowKey = new ByRowKeySpecification("rowKey");

            var entity = entityRepository.GetEntity(partitionKey, rowKey);

            //// make some change to entity ...

            entityRepository.Save(entity);

            context.Commit();
        }
    }
}
