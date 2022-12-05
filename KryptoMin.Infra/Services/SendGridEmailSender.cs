using KryptoMin.Application.Contracts;
using SendGrid.Helpers.Mail;
using SendGrid;
using KryptoMin.Infra.Exceptions;
using KryptoMin.Domain.Entities;
using KryptoMin.Infra.Abstract;
using KryptoMin.Application.Settings;

namespace KryptoMin.Infra.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IPdfReportGenerator _pdfReportGenerator;
        private readonly EmailSettings _emailSettings;
        private readonly IExcelReportGenerator _excelReportGenerator;

        public SendGridEmailSender(IPdfReportGenerator pdfReportGenerator, IExcelReportGenerator excelReportGenerator, EmailSettings emailSettings)
        {
            _pdfReportGenerator = pdfReportGenerator;
            _emailSettings = emailSettings;
            _excelReportGenerator = excelReportGenerator;
        }

        public async Task Send(string email, TaxReport report)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_emailSettings.Address, _emailSettings.Name),
                Subject = "KryptoMin Raport",
                PlainTextContent = "KryptoMin Raport",
                HtmlContent = _emailSettings.Content,
            };
            msg.AddTo(new EmailAddress(email));

            var response = await client.SendEmailAsync(msg);

            if(!response.IsSuccessStatusCode) 
            {
                throw new FailedToSendEmailException("Sending email with report failed");
            }
        }
    }
}