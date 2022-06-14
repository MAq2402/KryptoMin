namespace KryptoMin.Application.Models
{
    public class ExchangeRateRequest
    {
        public ExchangeRateRequest(string currency, string date)
        {
            Date = date;
            Currency = currency;
        }
        
        public string Date { get; }
        public string Currency { get; }
    }
}