using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IReportService
    {
        Task Send(SendReportRequestDto request);
    }
}