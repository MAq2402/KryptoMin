namespace KryptoMin.Application.Models
{
    public class ExchangeRate : ValueObject
    {
        public ExchangeRate(decimal value, string number, string date, string currency)
        {
            Value = value;
            Number = number;
            Date = date;
            Currency = currency;
        }

        public decimal Value { get;  }
        public string Number { get; }
        public string Date { get; }
        public string Currency { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Date;
            yield return Currency;
        }
    }
}