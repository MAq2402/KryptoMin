using CSharpFunctionalExtensions;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Entities
{
    public class Transaction : Entity
    {
        private const int Decimals = 2;

        public Transaction(Guid partitionKey, Guid rowKey, DateTime date, Amount amount, Amount fees, bool isSell) : base(partitionKey, rowKey)
        {
            Date = date;
            Amount = amount;
            Fees = fees;
            IsSell = isSell;
        }

        public Transaction(Guid partitionKey, Guid rowKey, DateTime date,
            Amount amount, Amount fees, bool isSell,
            ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees) : base(partitionKey, rowKey)
        {
            Date = date;
            Amount = amount;
            Fees = fees;
            IsSell = isSell;
            ExchangeRateForAmount = exchangeRateForAmount;
            ExchangeRateForFees = exchangeRateForFees;
        }

        public DateTime Date { get; }
        public Amount Amount { get; }
        public Amount Fees { get; }
        public bool IsSell { get; }
        public ExchangeRate ExchangeRateForAmount { get; private set; }
        public ExchangeRate ExchangeRateForFees { get; private set; }
        public bool HasFees => Fees != Amount.Zero;

        public void AssignExchangeRates(IEnumerable<ExchangeRate> exchangeRates)
        {
            ExchangeRateForAmount = FindExchangeRateForPreviousWorkingDay(exchangeRates, Amount.Currency);
            if (HasFees)
            {
                ExchangeRateForFees = FindExchangeRateForPreviousWorkingDay(exchangeRates, Fees.Currency);
            }
        }

        private ExchangeRate FindExchangeRateForPreviousWorkingDay(IEnumerable<ExchangeRate> exchangeRates, string currency)
        {
            return currency == ExchangeRate.DefaultCurrency ? ExchangeRate.Default :
                exchangeRates.Where(x => x.Date < Date).OrderByDescending(x => x.Date).First(x => x.Currency == currency);
        }

        public Result<decimal> CalculateProfits()
        {
            if (IsSell)
            {
                if (ExchangeRateForAmount is null)
                {
                    return Result.Failure<decimal>("Before calculating profits exchange rates for amount should be loaded.");
                }
                return Result.Success(Math.Round(Amount.Value * ExchangeRateForAmount.Value, Decimals));
            }
            else
            {
                return Result.Success(0.0m);
            }
        }

        public Result<decimal> CalculateCosts()
        {
            if (IsSell)
            {
                return FeesCosts();
            }
            else
            {
                if (ExchangeRateForAmount is null)
                {
                    return Result.Failure<decimal>("Before calculating profits exchange rates for amount should be loaded.");
                }
                var feesCostsResult = FeesCosts();
                return feesCostsResult.IsFailure ? feesCostsResult : 
                    Result.Success(Math.Round(Amount.Value * ExchangeRateForAmount.Value, Decimals) + feesCostsResult.Value);
            }
        }

        private Result<decimal> FeesCosts()
        {
            if (HasFees)
            {
                if (ExchangeRateForFees is null)
                {
                    return Result.Failure<decimal>("Before calculating costs exchange rates for fees should be loaded.");
                }
                return Result.Success(Math.Round(Fees.Value * ExchangeRateForFees.Value, Decimals));
            }
            else
            {
                return Result.Success(0.0m);
            }
        }
    }
}