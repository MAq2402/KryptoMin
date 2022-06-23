using KryptoMin.Infra.Models.Nbp;

namespace KryptoMin.Infra.HttpClients
{
    public interface INbpHttpClient
    {
        Task<ExchangeRatesResponse> Get(string currency, string date);
    }
}