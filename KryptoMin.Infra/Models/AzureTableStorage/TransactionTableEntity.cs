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
            Method = transaction.Method;
            Amount = transaction.Amount.ToString();
            Price = transaction.Price;
            Fees = transaction.Fees.ToString();
            FinalAmount = transaction.FinalAmount;
            IsSell = transaction.IsSell;
            TransactionId = transaction.TransactionId;
            ExchangeRateForAmount = transaction.ExchangeRateForAmount.ToString();
            ExchangeRateForFees = transaction.ExchangeRateForFees.ToString();
            Costs = Decimal.ToDouble(transaction.Costs);
            Profits = Decimal.ToDouble(transaction.Profits);
        }

        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string Fees { get; set; }
        public string FinalAmount { get; set; }
        public bool IsSell { get; set; }
        public string TransactionId { get; set; }
        public string ExchangeRateForAmount { get; set; }
        public string ExchangeRateForFees { get; set; }
        public double Profits { get; set; }
        public double Costs { get; set; }

        public Transaction ToDomain()
        {
            return new Transaction(new Guid(PartitionKey), 
                new Guid(RowKey), Date, Method, new Amount(Amount), Price, 
                new Amount(Fees), FinalAmount, IsSell, TransactionId, 
                (ExchangeRate)ExchangeRateForAmount, (ExchangeRate)ExchangeRateForFees, 
                Convert.ToDecimal(Profits), Convert.ToDecimal(Costs));
        }
    }
}