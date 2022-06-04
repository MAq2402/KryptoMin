namespace KryptoMin.Application.Contracts
{
    public class Purchase
    {
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public ExchangeRate ExchangeRate { get; set; }
        public string FormattedDate => Date.ToString("yyyy-MM-dd");
        public bool FailedToGetExchangeRate { get; set; } = false;
    }

    public class ExchangeRate
    {
        public double Value { get; set; }
        public string Number { get; set; }
    }

    public interface ICurrencyProvider
    {
        Task<IEnumerable<Purchase>> Get(IEnumerable<Purchase> purchases);
    }
}