using KryptoMin.Application.Models;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRateProvider
    {
        Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequest> requests);
    }
}