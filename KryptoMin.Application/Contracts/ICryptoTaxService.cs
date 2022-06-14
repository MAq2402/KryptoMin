using KryptoMin.Application.Models;

namespace KryptoMin.Application.Contracts
{
    public interface ICryptoTaxService
    {
        Task<TaxReportDto> GenerateReport(IEnumerable<PurchaseDto> purchases);
    }
}