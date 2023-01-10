using KryptoMin.Application.Settings;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Abstract
{
    public abstract class AzureTableStorageRepository
    {
        private CloudStorageAccount _storageAccount;

        public AzureTableStorageRepository(DbSettings dbSettings)
        {
            _storageAccount = CloudStorageAccount.Parse(dbSettings.ConnectionString);
        }

        protected CloudTable CreateCloudTableClient(string tableName)
        {
            return _storageAccount.CreateCloudTableClient(new TableClientConfiguration()).GetTableReference(tableName);
        }
    }
}