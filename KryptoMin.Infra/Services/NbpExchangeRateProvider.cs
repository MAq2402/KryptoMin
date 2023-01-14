using System.Globalization;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.ValueObjects;
using KryptoMin.Infra.HttpClients;

namespace KryptoMin.Infra.Services
{
    public class NbpExchangeRateProvider : IExchangeRatesProvider
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

        private bool CheckIfUniqueExchangeRate(List<ExchangeRate> rates, string currency, DateTime date)
        {
            return !rates.Any(x => x.Currency == currency && x.Date == date);
        }

        private async Task<ExchangeRate> Get(string currency, DateTime date)
        {
            if (currency == PLN)
            {
                return new ExchangeRate(1, string.Empty, date, PLN);
            }
            else
            {
                var response = await _httpClient.Get(currency, date.ToString("yyyy-MM-dd"));

                return new ExchangeRate(response.Rates.First().Mid,
                    response.Rates.First().No, date, currency);
            }
        }
    }
}