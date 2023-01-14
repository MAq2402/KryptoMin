using KryptoMin.Domain.Entities;
using KryptoMin.Infra.Abstract;
using ClosedXML.Excel;
using System.Reflection;

namespace KryptoMin.Infra.Services
{
    public class ExcelReportGenerator : IExcelReportGenerator
    {
        public string Generate(TaxReport report)
        {
            using (var workbook = new XLWorkbook())
            {
                var properties = typeof(TaxReport).GetProperties();
                var worksheet = workbook.Worksheets.Add("KryptoMinReport");

                worksheet = WriteHeader(properties, worksheet);
                worksheet = WriteContent(report, properties, worksheet);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return Convert.ToBase64String(stream.ToArray());
                }
            }
        }

        private IXLWorksheet WriteHeader(PropertyInfo[] properties, IXLWorksheet worksheet)
        {
            var headerRow = 1;
            var currentColumn = 1;
            foreach (var property in properties)
            {
                worksheet.Cell(headerRow, currentColumn).Value = property.Name;
                currentColumn++;
            }
            return worksheet;
        }

        private IXLWorksheet WriteContent(TaxReport data, PropertyInfo[] properties, IXLWorksheet worksheet)
        {
            var currentColumn = 1;
            var currentRow = 2;
            foreach (var property in properties)
            {
                worksheet.Cell(currentRow, currentColumn).Value = property.GetValue(data)?.ToString();
                currentColumn++;
            }
            currentRow++;
            currentColumn = 1;

            return worksheet;
        }
    }
}