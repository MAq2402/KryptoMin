using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface ICryptoTaxService
    {
        Task<TaxReportDto> GenerateReport(TaxReportRequestDto request);
    }
}