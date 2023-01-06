using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRatesRepository
    {
        //Consider different type for exchangerate
        Task<ExchangeRate> GetForPreviousWorkingDay(string currency, DateTime date);
        Task Insert(IEnumerable<NbpCsvExchnageRateDto> exchangeRates);
        Task RemoveAll();
    }
}