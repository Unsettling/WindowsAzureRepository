namespace AzurePatterns.Interfaces
{
    public interface IUnitOfWork
    {
        void Commit();

        void Rollback();
    }
}