using KryptoMin.Application.Dtos;
using KryptoMin.Application.Models;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRateProvider
    {
        Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequestDto> requests);
    }
}