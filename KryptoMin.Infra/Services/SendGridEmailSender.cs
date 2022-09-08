using KryptoMin.Application.Contracts;
using SendGrid.Helpers.Mail;
using SendGrid;
using KryptoMin.Infra.Exceptions;
using KryptoMin.Domain.Entities;

namespace KryptoMin.Infra.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        public async Task Send(string email, TaxReport report)
        {
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("maq0297@gmail.com", "Michal Miciak"),
                Subject = $"Tax: {report.Balance}",
                PlainTextContent = "and easy to do anywhere, especially with C#",
                HtmlContent = "and easy to do anywhere, <strong>especially with C#</strong>"
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