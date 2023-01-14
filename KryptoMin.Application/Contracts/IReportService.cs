using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IReportService
    {
        Task<ReportResponseDto> Send(SendReportRequestDto request);
        Task<GenerateResponseDto> Generate(GenerateRequestDto request);
    }
}