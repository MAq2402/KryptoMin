namespace KryptoMin.Application.Models
{
    public class Purchase
    {
        public Purchase(DateTime date, string currency)
        {
            Date = date;
            Currency = currency;    
            FailedToGetExchangeRate = false;
        }

        public DateTime Date { get; }
        public string Currency { get; }
        public ExchangeRate ExchangeRate { get; private set; }
        public string FormattedDate => Date.ToString("yyyy-MM-dd");
        public bool FailedToGetExchangeRate { get; private set; }

        public Purchase FailToGetExchangeRate()
        {
            var result = new Purchase(Date, Currency);
            result.FailedToGetExchangeRate = true;
            return result;
        }

        public Purchase SetExchangeRate(double value, string number)
        {
            var result = new Purchase(Date, Currency);
            result.ExchangeRate = new ExchangeRate(value, number);
            return result;
        }
    }
}