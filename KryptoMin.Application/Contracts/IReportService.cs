using CSharpFunctionalExtensions;
using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IReportService
    {
        Task<ReportResponseDto> Send(SendReportRequestDto request);
        Task<Result<GenerateResponseDto>> Generate(GenerateRequestDto request);
    }
}