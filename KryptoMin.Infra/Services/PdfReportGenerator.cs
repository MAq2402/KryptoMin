using KryptoMin.Domain.Entities;
using KryptoMin.Infra.Abstract;

namespace KryptoMin.Infra.Services
{
    public class PdfReportGenerator : IPdfReportGenerator
    {
        public string Generate(TaxReport report)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var html = string.Empty;
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}