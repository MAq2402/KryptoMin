using KryptoMin.Infra.Models.Nbp;
using Newtonsoft.Json;

namespace KryptoMin.Infra.HttpClients
{
    public class NbpHttpClient
    {
        private readonly HttpClient _httpClient;

        public NbpHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri("http://api.nbp.pl/api/");
        }

        public async Task<ExchangeRatesResponse> Get(string currency, string date)
        {
            var url = $"exchangerates/rates/a/{currency}/{date}/?format=json";
            return JsonConvert.DeserializeObject<ExchangeRatesResponse>(await _httpClient.GetStringAsync(url));
        }
    }
}