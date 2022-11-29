using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IReportService
    {
        Task<TaxReportResponseDto> Send(SendReportRequestDto request);
        Task<TaxReportResponseDto> Get(GetReportRequestDto request);
        Task<TaxReportResponseDto> GenerateReport(TaxReportRequestDto request);
    }
}