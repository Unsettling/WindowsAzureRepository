namespace AzurePatterns.TableStorage
{
    using System;

    using AzurePatterns.Interfaces;

    using Microsoft.WindowsAzure.StorageClient;

    public class RepositoryBase
    {
        public RepositoryBase(IUnitOfWork context, string table)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("table", "Expected a table name.");
            }

            this.Context = context as TableServiceContext;
            this.Table = table;

            // belt-and-braces code - ensure the table is there for the repository.
            if (this.Context != null)
            {
                var cloudTableClient = new CloudTableClient(this.Context.BaseUri, this.Context.StorageCredentials);
                cloudTableClient.CreateTableIfNotExist(this.Table);
            }
        }

        protected TableServiceContext Context { get; private set; }

        protected string Table { get; private set; }
    }
}
