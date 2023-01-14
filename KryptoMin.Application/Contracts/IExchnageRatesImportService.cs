namespace KryptoMin.Application.Contracts
{
    public interface IExchangeRatesImportService
    {
        Task Import(Stream stream);
    }
}