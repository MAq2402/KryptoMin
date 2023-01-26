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
            PreviousYearsCosts = Decimal.ToDouble(report.PreviousYearsCosts);
            OwnerEmail = report.OwnerEmail;
            Status = (int)report.Status;
            Revenue = Decimal.ToDouble(report.Revenue);
            Costs = Decimal.ToDouble(report.Costs);
            Income = Decimal.ToDouble(report.Income);
            CurrentYearCosts =  Decimal.ToDouble(report.CurrentYearCosts);
            Tax = Decimal.ToDouble(report.Tax);
        }

        public double Revenue { get; set; }
        public double Costs { get; set; }
        public double PreviousYearsCosts { get; set; }
        public double Income { get; set; }
        public double CurrentYearCosts { get; set; }
        public double Tax { get; set; }
        public string OwnerEmail { get; set; }
        public int Status { get; set; }

        public TaxReport ToDomain(List<Transaction> transactions)
        {
            return new TaxReport(new Guid(PartitionKey), new Guid(RowKey), transactions, (decimal)PreviousYearsCosts, OwnerEmail, 
                (TaxReportStatus)Status, (decimal)Revenue, (decimal)Costs, (decimal)Income, (decimal)CurrentYearCosts, (decimal)Tax);
        } 
    }
}