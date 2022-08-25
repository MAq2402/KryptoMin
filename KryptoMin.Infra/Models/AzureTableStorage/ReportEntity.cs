using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Models.AzureTableStorage
{
    public class ReportEntity : TableEntity
    {
        public ReportEntity()
        {
        }

        public ReportEntity(string transactions, decimal balance, decimal balanceWithPreviousYearLoss, decimal tax, decimal previousYearLoss)
        {
            PartitionKey = Guid.NewGuid().ToString();
            RowKey = Guid.NewGuid().ToString();
            Transactions = transactions;
            Balance = balance;
            BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss;
            Tax = tax;
            PreviousYearLoss = previousYearLoss;
            isSent = false;
        }
        public string Transactions { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithPreviousYearLoss { get; set; }
        public decimal Tax { get; set; }
        public decimal PreviousYearLoss { get; set; }
        public string Email { get; set; }
        public bool isSent { get; set; }
    }
}