using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;
using KryptoMin.Infra.HttpClients;

namespace KryptoMin.Infra.Services
{
    public class NbpExchangeRateProvider : IExchangeRateProvider
    {
        private const string PLN = "PLN";
        private readonly INbpHttpClient _httpClient;

        public NbpExchangeRateProvider(INbpHttpClient client)
        {
            _httpClient = client;
        }

        public async Task<IEnumerable<ExchangeRate>> Get(IEnumerable<ExchangeRateRequestDto> requests)
        {
            var result = new List<ExchangeRate>();
            foreach (var reqeust in requests)
            {
                if (CheckIfUniqueExchangeRate(result, reqeust.Currency, reqeust.Date))
                    result.Add(await Get(reqeust.Currency, reqeust.Date));
            }

            return result;
        }

        private bool CheckIfUniqueExchangeRate(List<ExchangeRate> rates, string currency, string date)
        {
            return !rates.Any(x => x.Currency == currency && x.Date == date);
        }

        private async Task<ExchangeRate> Get(string currency, string date)
        {
            if (currency == PLN)
            {
                return new ExchangeRate(1, string.Empty, date, PLN);
            }
            else
            {
                var response = await _httpClient.Get(currency, date);

                return new ExchangeRate(response.Rates.First().Mid,
                    response.Rates.First().No, date, currency);
            }
        }
    }
}