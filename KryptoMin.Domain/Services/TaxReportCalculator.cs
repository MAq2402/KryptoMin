using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Enums;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Services
{
    public class TaxReportCalculator : ITaxReportCalculator
    {
        private const decimal TaxRate = 0.19m;
        private const int DecimalPlaces = 2;
        public TaxReport Calculate(IEnumerable<Transaction> transactions, 
            IEnumerable<ExchangeRate> exchangeRates, Guid reportPartitionId, decimal previousYearLoss)
        {
            var balance = 0.0m;
            foreach (var transaction in transactions)
            {
                var exchangeRateForAmount = GetExchangeRate(exchangeRates, transaction.Amount.Currency, transaction.FormattedPreviousWorkingDay);
                var exchangeRateForFees = GetExchangeRate(exchangeRates, transaction.Fees.Currency, transaction.FormattedPreviousWorkingDay);

                transaction.SetExchangeRates(exchangeRateForAmount, exchangeRateForFees);
                balance -= transaction.CalculateCosts();
                balance += transaction.CalculateProfits();
            }

            balance = Math.Round(balance, DecimalPlaces);
            var balanceWithPreviousYearLoss = Math.Round(balance - previousYearLoss, DecimalPlaces);
            var tax = balanceWithPreviousYearLoss > 0 ? Math.Round(balanceWithPreviousYearLoss * TaxRate, DecimalPlaces) : 0;
  
            return new TaxReport(reportPartitionId, Guid.NewGuid(), 
                transactions, balance, balanceWithPreviousYearLoss, tax, previousYearLoss);
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.Date == date);
        }
    }
}