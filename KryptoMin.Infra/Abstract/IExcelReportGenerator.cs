using KryptoMin.Domain.Entities;

namespace KryptoMin.Infra.Abstract
{
    public interface IExcelReportGenerator
    {
        string Generate(TaxReport report);    
    }
}