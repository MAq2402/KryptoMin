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
            ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees) : base(partitionKey, rowKey)
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
        public bool HasFees => Fees != Amount.Zero;

        // public void AssignExchangeRateForAmount(ExchangeRate exchangeRate)
        // {
        //     ExchangeRateForAmount = exchangeRate;
        // }

        // public void AssignExchangeRateForFees(ExchangeRate exchangeRate)
        // {
        //     ExchangeRateForFees = exchangeRate;
        // }

        public void AssignExchangeRates(IEnumerable<ExchangeRate> exchangeRates)
        {
            ExchangeRateForAmount = GetExchangeRate(exchangeRates, Amount.Currency, FormattedPreviousWorkingDay);
            if (HasFees)
            {
                ExchangeRateForFees = GetExchangeRate(exchangeRates, Fees.Currency, FormattedPreviousWorkingDay);
            }
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.FormattedDate == date);
        }

        public decimal CalculateProfits()
        {
            if (IsSell)
            {
                if (ExchangeRateForAmount is null)
                {
                    throw new InvalidOperationException("Before calculating profits exchange rates for amount should be loaded.");
                }
                return Math.Round(Amount.Value * ExchangeRateForAmount.Value, 2);
            }
            else
            {
                return 0m;
            }
        }

        public decimal CalculateCosts()
        {
            if (IsSell)
            {
                return FeesCosts();
            }
            else {
                if (ExchangeRateForAmount is null)
                {
                    throw new InvalidOperationException("Before calculating profits exchange rates for amount should be loaded.");
                }
                return Math.Round(Amount.Value * ExchangeRateForAmount.Value, 2) + FeesCosts();
            }
        }
        
        private decimal FeesCosts()
        {
            if (HasFees)
            {
                if (ExchangeRateForFees is null)
                {
                    throw new InvalidOperationException("Before calculating costs exchange rates for fees should be loaded.");
                }
                return Math.Round(Fees.Value * ExchangeRateForFees.Value, 2);
            }
            else {
                return 0.0m;
            }
        }
    }
}