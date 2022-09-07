using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface ICryptoTaxService
    {
        Task<TaxReportResponseDto> GenerateReport(TaxReportRequestDto request);
    }
}