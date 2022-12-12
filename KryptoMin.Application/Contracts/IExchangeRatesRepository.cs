using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRatesRepository
    {
        Task Insert(IEnumerable<NbpCsvExchnageRateDto> exchangeRates);
        Task RemoveAll();
    }
}