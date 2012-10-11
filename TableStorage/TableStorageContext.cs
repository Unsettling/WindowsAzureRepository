namespace AzurePatterns.TableStorage
{
    using System.Data.Services.Client;
    using System.Net;

    using AzurePatterns.Interfaces;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class TableStorageContext : TableServiceContext, IUnitOfWork
    {
        // Constructor allows for setting up a specific connection string (for testing).
        public TableStorageContext(string connectionString = null)
            : base(BaseAddress(connectionString), CloudCredentials(connectionString))
        {
            this.SetupContext();
        }

        // NOTE: the implementation of Commit may vary depending on your desired table behaviour.
        public void Commit()
        {
            try
            {
                // Insert or Merge Entity aka Upsert (>=v.1.4) uses 
                // SaveChangesOptions.None to generate a merge request.
                this.SaveChanges(SaveChangesOptions.None);
            }
            catch (DataServiceRequestException exception)
            {
                var dataServiceClientException = exception.InnerException as DataServiceClientException;
                if (dataServiceClientException != null)
                {
                    if (dataServiceClientException.StatusCode == (int)HttpStatusCode.Conflict)
                    {
                        // a conflict may arise on a retry where it succeeded so this is ignored.
                        // TODO: this should be a list of codes to check and ignore/log/retry etc.
                        return;
                    }
                }

                throw;
            }
        }

        public void Rollback()
        {
            // TODO: clean up context.
        }

        private static string BaseAddress(string connectionString)
        {
            return CloudStorageAccount(connectionString).TableEndpoint.ToString();
        }

        private static StorageCredentials CloudCredentials(string connectionString)
        {
            return CloudStorageAccount(connectionString).Credentials;
        }

        private static CloudStorageAccount CloudStorageAccount(string connectionString)
        {
            var cloudConnectionString = connectionString ?? CloudConfigurationManager.GetSetting("CloudConnectionString");
            var cloudStorageAccount = Microsoft.WindowsAzure.CloudStorageAccount.Parse(cloudConnectionString);
            return cloudStorageAccount;
        }

        private void SetupContext()
        {
            /*
             * this retry policy will introduce a greater delay if there are retries 
             * than the original setting of 3 retries in 3 seconds but it will then 
             * show up a problem with the system without the system failing completely.
             */
            this.RetryPolicy = RetryPolicies.RetryExponential(
                RetryPolicies.DefaultClientRetryCount, RetryPolicies.DefaultClientBackoff);

            // don't throw a DataServiceRequestException when a row doesn't exist.
            this.IgnoreResourceNotFoundException = true;
        }
    }
}
