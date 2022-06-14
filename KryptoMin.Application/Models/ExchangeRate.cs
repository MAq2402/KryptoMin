namespace KryptoMin.Application.Models
{
    public class ExchangeRate
    {
        public ExchangeRate(double value, string number, string date, string currency)
        {
            Value = value;
            Number = number;
            Date = date;
            Currency = currency;
        }

        public double Value { get;  }
        public string Number { get; }
        public string Date { get; }
        public string Currency { get; }
    }
}