using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Services
{
    public interface ITaxReportCalculator
    {
        TaxReport Calculate(IEnumerable<Transaction> transactions, IEnumerable<ExchangeRate> exchangeRates, Guid reportPartitionId, decimal previousYearLoss);
    }
}