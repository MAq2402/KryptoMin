using KryptoMin.Application.Contracts;
using Newtonsoft.Json;

namespace KryptoMin.Infra.Services
{

    public class Rates
    {
        public double Mid { get; set; }
        public string No { get; set; }
    }

    public class NbpExchangeRatesResponse
    {
        public IEnumerable<Rates> Rates { get; set; }
    }
    public class NbpCurrencyProvider : ICurrencyProvider
    {
        private HttpClient _httpClient;

        public NbpCurrencyProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://api.nbp.pl/api/");
        }

        public async Task<IEnumerable<Purchase>> Get(IEnumerable<Purchase> purchases)
        {
            var result = new List<Purchase>();
            foreach (var purchase in purchases)
            {
                var url = $"exchangerates/rates/a/{purchase.Currency.ToLower()}/{purchase.FormattedDate}/?format=json";
                var response = JsonConvert.DeserializeObject<NbpExchangeRatesResponse>(await _httpClient.GetStringAsync(url));

                if (response is null || !response.Rates.Any())
                {
                    purchase.FailedToGetExchangeRate = true;
                }
                else
                {
                    var rate = response.Rates.First();
                    purchase.ExchangeRate = new ExchangeRate
                    {
                        Number = rate.No,
                        Value = rate.Mid
                    };
                    result.Add(purchase);
                }
            }

            return result;
        }
    }
}