using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Infra.Services
{
    public class NbpImportedExchangeRatesProvider : IExchangeRatesProvider
    {
        private IExchangeRatesRepository _exchangeRatesRepository;

        public NbpImportedExchangeRatesProvider(IExchangeRatesRepository exchangeRatesRepository)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        public async Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequestDto> requests)
        {
            return await _exchangeRatesRepository.GetExchangeRates(requests.Where(x => x.Currency != ExchangeRate.DefaultCurrency).DistinctBy(x => new { x.Currency, x.Date }));
        }
    }
}