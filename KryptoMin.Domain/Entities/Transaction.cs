 using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Entities
{
    public class Transaction : Entity
    {
        public Transaction(Guid partitionKey, Guid rowKey, DateTime date, 
            string method, Amount amount, string price, Amount fees, 
            string finalAmount, bool isSell, string transactionId) : base(partitionKey, rowKey)
        {
            Date = date;
            Method = method;
            Amount = amount;
            Price = price;
            Fees = fees;
            FinalAmount = finalAmount;
            IsSell = isSell; 
            TransactionId = transactionId;
        }


        public Transaction(Guid partitionKey, Guid rowKey, DateTime date, 
            string method, Amount amount, string price, Amount fees, 
            string finalAmount, bool isSell, string transactionId, 
            ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, 
            decimal profits, decimal costs) : base(partitionKey, rowKey)
        {
            Date = date;
            Method = method;
            Amount = amount;
            Price = price;
            Fees = fees;
            FinalAmount = finalAmount;
            IsSell = isSell; 
            TransactionId = transactionId;
            ExchangeRateForAmount = exchangeRateForAmount;
            ExchangeRateForFees = exchangeRateForFees;
            Profits = profits;
            Costs = costs;
        }

        public DateTime Date { get; }
        public DateTime PreviousWorkingDay => Date.DayOfWeek == DayOfWeek.Monday ? Date.AddDays(-3) : 
            Date.DayOfWeek == DayOfWeek.Sunday ? Date.AddDays(-2) : Date.AddDays(-1);
        public string FormattedPreviousWorkingDay => PreviousWorkingDay.ToString("yyyy-MM-dd");
        public string Method { get; }
        public Amount Amount { get; }
        public string Price { get; }
        public Amount Fees { get; }
        public string FinalAmount { get; }
        public bool IsSell { get; }
        public string TransactionId { get; }
        public ExchangeRate ExchangeRateForAmount { get; private set; }
        public ExchangeRate ExchangeRateForFees { get; private set; }
        public decimal Profits { get; private set; }
        public decimal Costs { get; private set; }
        public bool HasFees => Fees != Amount.Zero;

        public void SetExchangeRates(ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees)
        {
            ExchangeRateForAmount = exchangeRateForAmount;
            ExchangeRateForFees = exchangeRateForFees;
        }

        public void CalculateProfits()
        {
            if (IsSell)
            {
                if (ExchangeRateForAmount is null)
                {
                    throw new InvalidOperationException("Before calculating profits exchange rates should be loaded.");
                }
                Profits = Amount.Value * ExchangeRateForAmount.Value;
            }
            else
            {
                Profits = 0m;
            }
        }

        public void CalculateCosts()
        {
            if (IsSell)
            {
                Costs = FeesCosts();
            }
            else {
                if (ExchangeRateForAmount is null)
                {
                    throw new InvalidOperationException("Before calculating profits exchange rates should be loaded.");
                }
                Costs = Amount.Value * ExchangeRateForAmount.Value + FeesCosts();
            }
        }
        
        private decimal FeesCosts()
        {
            if (HasFees)
            {
                if (ExchangeRateForFees is null)
                {
                    throw new InvalidOperationException("Before calculating costs exchange rates should be loaded.");
                }
                return Fees.Value * ExchangeRateForFees.Value;
            }
            else {
                return 0.0m;
            }
        }
    }
}