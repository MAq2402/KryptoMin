using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Enums;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Models.AzureTableStorage
{
    public class TaxReportTableEntity : TableEntity
    {
        public TaxReportTableEntity()
        {
        }
        
        public TaxReportTableEntity(TaxReport report)
        {
            PartitionKey = report.PartitionKey.ToString();
            RowKey = report.RowKey.ToString();
            Balance = Decimal.ToDouble(report.Balance);
            BalanceWithPreviousYearLoss = Decimal.ToDouble(report.BalanceWithPreviousYearLoss);
            Tax = Decimal.ToDouble(report.Tax);
            PreviousYearLoss = Decimal.ToDouble(report.PreviousYearLoss);
            OwnerEmail = report.OwnerEmail;
            Status = (int)report.Status;
        }

        public double Balance { get; set; }
        public double BalanceWithPreviousYearLoss { get; set; }
        public double Tax { get; set; }
        public double PreviousYearLoss { get; set; }
        public string OwnerEmail { get; set; }
        public int Status { get; set; }

        public TaxReport ToDomain(List<Transaction> transactions)
        {
            return new TaxReport(new Guid(PartitionKey), new Guid(RowKey), transactions, (decimal)Balance,
                (decimal)BalanceWithPreviousYearLoss, (decimal)Tax, (decimal)PreviousYearLoss, OwnerEmail, (TaxReportStatus)Status);
        } 
    }
}