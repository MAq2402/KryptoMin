using KryptoMin.Domain.Entities;

namespace KryptoMin.Application.Contracts
{
    public interface IEmailSender
    {
        Task Send(string email, TaxReport taxReport);
    }
}