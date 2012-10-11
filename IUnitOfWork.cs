namespace AzurePatterns
{
    public interface IUnitOfWork
    {
        void Commit();

        void Rollback();
    }
}