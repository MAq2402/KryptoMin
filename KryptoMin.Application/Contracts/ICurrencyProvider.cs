using KryptoMin.Application.Models;

namespace KryptoMin.Application.Contracts
{
    public interface ICurrencyProvider
    {
        Task<IEnumerable<Purchase>> Get(IEnumerable<Purchase> purchases);
    }
}