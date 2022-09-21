using KryptoMin.Domain.Entities;

namespace KryptoMin.Infra.Abstract
{
    public interface IPdfReportGenerator
    {
        string Generate(TaxReport report);
    }
}