using KryptoMin.Domain.Entities;
using KryptoMin.Infra.Abstract;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace KryptoMin.Infra.Services
{
    public class PdfReportGenerator : IPdfReportGenerator
    {
        public string Generate(TaxReport report)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var html = string.Format(@"<body><span>Saldo: {0} zł</span>
                        <span>Saldo ze stratą z poprzedniego roku: {1} zł</span>
                        <span>Strata z poprzedniego roku: {2} zł</span>
                        <span>Podatek: {3} zł</span></body>", report.Balance, report.BalanceWithPreviousYearLoss, report.PreviousYearsCosts, report.Tax);
                // //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // var pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                // pdf.AddPage();
                // pdf.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}