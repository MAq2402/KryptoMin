using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRatesProvider
    {
        Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequestDto> requests);
    }
}