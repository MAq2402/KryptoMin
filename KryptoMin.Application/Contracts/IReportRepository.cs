using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Contracts
{
    public interface IReportRepository
    {
        Task Save(TaxReportDto report);
    }
}