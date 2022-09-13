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

        public SendGridEmailSender(IPdfReportGenerator pdfReportGenerator, EmailSettings emailSettings)
        {
            _pdfReportGenerator = pdfReportGenerator;
            _emailSettings = emailSettings;
        }

        public async Task Send(string email, TaxReport report)
        {
            var pdf = _pdfReportGenerator.Generate(report);
            var client = new SendGridClient(_emailSettings.ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("maq0297@gmail.com", "Michal Miciak"),
                Subject = $"KryptoMin Report",
                PlainTextContent = "",
                HtmlContent = "",
            };
            msg.AddAttachment("KryptoMin_Raport.pdf", pdf);
            msg.AddTo(new EmailAddress(email));

            var response = await client.SendEmailAsync(msg);

            if(!response.IsSuccessStatusCode) 
            {
                throw new FailedToSendEmailException("Sending email with report failed");
            }
        }
    }
}