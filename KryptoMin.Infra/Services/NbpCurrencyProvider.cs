using KryptoMin.Application.Contracts;
using KryptoMin.Application.Models;
using KryptoMin.Infra.HttpClients;

namespace KryptoMin.Infra.Services
{
    public class NbpCurrencyProvider : ICurrencyProvider
    {
        private readonly NbpHttpClient _httpClient;

        public NbpCurrencyProvider(NbpHttpClient client)
        {
            _httpClient = client;
        }

        public async Task<IEnumerable<Purchase>> Get(IEnumerable<Purchase> purchases)
        {
            var result = new List<Purchase>();
            foreach (var purchase in purchases)
            {
                var url = $"exchangerates/rates/a/{purchase.Currency.ToLower()}/{purchase.FormattedDate}/?format=json";
                var response = await _httpClient.Get(purchase.Currency.ToLower(), purchase.FormattedDate);

                if (response is null || !response.Rates.Any())
                {
                    result.Add(purchase.FailToGetExchangeRate());
                }
                else
                {
                    var rate = response.Rates.First();   
                    result.Add(purchase.SetExchangeRate(rate.Mid, rate.No));
                }
            }

            return result;
        }
    }
}