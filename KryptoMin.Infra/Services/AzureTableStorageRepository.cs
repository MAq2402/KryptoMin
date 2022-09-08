using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Enums;
using KryptoMin.Domain.Repositories;
using KryptoMin.Infra.Models.AzureTableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Services
{
    public class AzureTableStorageRepository : IRepository<TaxReport>
    {
        private readonly CloudTable _reports;
        private readonly CloudTable _transactions;

        public AzureTableStorageRepository()
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            _reports = storageAccount.CreateCloudTableClient(new TableClientConfiguration()).GetTableReference("Reports");
            _transactions = storageAccount.CreateCloudTableClient(new TableClientConfiguration()).GetTableReference("Transactions");
        }

        public async Task<TaxReport> Get(Guid partitionKey, Guid rowKey)
        {
            var transactionsQuery = new TableQuery<TransactionTableEntity>().Where(
                            TableQuery.GenerateFilterCondition(nameof(TransactionTableEntity.PartitionKey), QueryComparisons.Equal, partitionKey.ToString()));
            var transactions = _transactions.ExecuteQuery<TransactionTableEntity>(transactionsQuery).Select(x => x.ToDomain());
        
            var retrieveReport = TableOperation.Retrieve<TaxReportTableEntity>(partitionKey.ToString(), rowKey.ToString());
            var taxReport = await _reports.ExecuteAsync(retrieveReport);

            return (taxReport.Result as TaxReportTableEntity).ToDomain(transactions.ToList());
        }

        public async Task Add(TaxReport report)
        {
            var insertReport = TableOperation.Insert(new TaxReportTableEntity(report));
            var batchTransactionInsert = new TableBatchOperation();

            foreach(var transaction in report.Transactions)
            {
                batchTransactionInsert.Add(TableOperation.Insert(new TransactionTableEntity(transaction)));
            }
            
            await _reports.ExecuteAsync(insertReport);
            await _transactions.ExecuteBatchAsync(batchTransactionInsert);
        }

        public async Task Update(TaxReport report)
        {
            var tableEntity = new TaxReportTableEntity(report);
            tableEntity.ETag = "*";
            var replace = TableOperation.Replace(tableEntity);
            await _reports.ExecuteAsync(replace);
        }
    }
}
