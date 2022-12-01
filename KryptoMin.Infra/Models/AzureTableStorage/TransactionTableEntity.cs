using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Models.AzureTableStorage
{
    public class TransactionTableEntity : TableEntity
    {
        public TransactionTableEntity()
        {
        }
        
        public TransactionTableEntity(Transaction transaction)
        {
            PartitionKey = transaction.PartitionKey.ToString();
            RowKey = transaction.RowKey.ToString();
            Date = transaction.Date;
            Amount = transaction.Amount.ToString();
            Fees = transaction.Fees.ToString();
            IsSell = transaction.IsSell;
            ExchangeRateForAmount = transaction.ExchangeRateForAmount.ToString();
            ExchangeRateForFees = transaction.HasFees ? transaction.ExchangeRateForFees.ToString() : string.Empty;
        }

        public DateTime Date { get; set; }
        public string Amount { get; set; }
        public string Fees { get; set; }
        public bool IsSell { get; set; }
        public string ExchangeRateForAmount { get; set; }
        public string ExchangeRateForFees { get; set; }

        public Transaction ToDomain()
        {
            return new Transaction(new Guid(PartitionKey), 
                new Guid(RowKey), Date, new Amount(Amount),
                string.IsNullOrEmpty(Fees) ? Domain.ValueObjects.Amount.Zero : new Amount(Fees), IsSell, 
                (ExchangeRate)ExchangeRateForAmount, (ExchangeRate)ExchangeRateForFees);
        }
    }
}