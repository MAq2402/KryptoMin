using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRatesRepository
    {
        Task Insert(IEnumerable<NbpCsvExchnageRateDto> exchangeRates);
        Task RemoveAll();
        Task<List<ExchangeRate>> GetExchangeRates(IEnumerable<ExchangeRateRequestDto> requests);
    }
}