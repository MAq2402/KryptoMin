using System.Text.Json;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Infra.Models.AzureTableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Services
{
    public class AzureTableStorageRepository : IReportRepository
    {
        public async Task Save(TaxReportDto report)
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            var tableName = "Reports";

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var table = storageAccount.CreateCloudTableClient(new TableClientConfiguration()).GetTableReference(tableName);

            var insert = TableOperation.Insert(new ReportEntity(
                JsonSerializer.Serialize(report.Transactions), report.Balance, report.BalanceWithPreviousYearLoss, report.Tax, report.PreviousYearLoss));
            
            await table.ExecuteAsync(insert);
        }
    }
}
