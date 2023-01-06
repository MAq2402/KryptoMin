using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Infra.Services
{
    public class NbpImportedExchangeRatesProvider : IExchangeRatesProvider
    {
        private const string PLN = "PLN";
        private IExchangeRatesRepository _exchangeRatesRepository;

        public NbpImportedExchangeRatesProvider(IExchangeRatesRepository exchangeRatesRepository)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        public async Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequestDto> requests)
        {
            var result = new List<ExchangeRate>();
            foreach(var request in requests) 
            {
                if (request.Currency == PLN)
                {
                    result.Add(new ExchangeRate(1, string.Empty, request.Date, PLN));
                } else {
                    result.Add(await _exchangeRatesRepository.GetForPreviousWorkingDay(request.Currency, request.Date));
                }
            }

            return result;
        }
    }
}