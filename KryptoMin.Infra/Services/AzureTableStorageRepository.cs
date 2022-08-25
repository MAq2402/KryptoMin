using System.Text.Json;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Services
{
    public class ReportEntity : TableEntity
    {
        public ReportEntity()
        {

        }
        public ReportEntity(string transactions, decimal balance, decimal balanceWithPreviousYearLoss, decimal tax, decimal previousYearLoss, string email)
        {
            PartitionKey = Guid.NewGuid().ToString();
            RowKey = Guid.NewGuid().ToString();
            Transactions = transactions;
            Balance = balance;
            BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss;
            Tax = tax;
            PreviousYearLoss = previousYearLoss;
            Email = email;
            Sent = false;
        }
        public string Transactions { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithPreviousYearLoss { get; set; }
        public decimal Tax { get; set; }
        public decimal PreviousYearLoss { get; set; }
        public string Email { get; set; }
        public bool Sent { get; set; }
    }
    public class AzureTableStorageRepository : IReportRepository
    {
        public async Task Save(TaxReportDto report)
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=kryptomindev;AccountKey=ibubeZAUH7zjoFLB/doTB+6AacvNXqW7FmsCugho24BXD9OgHJR8IbzxOT3bFy2Fv+nCU2IbbDFQ+AStIlSK7g==;EndpointSuffix=core.windows.net";
            var tableName = "Reports";

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);

            var insert = TableOperation.Insert(new ReportEntity(
                JsonSerializer.Serialize(report.Transactions), report.Balance, report.BalanceWithPreviousYearLoss, report.Tax, report.PreviousYearLoss, ""));
            
            await table.ExecuteAsync(insert);
                
        }
    }
}
