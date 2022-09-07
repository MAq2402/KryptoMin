namespace KryptoMin.Domain.ValueObjects
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

        public override string ToString()
        {
            return $"{Value},{Number},{Date},{Currency}";
        }

        public static explicit operator ExchangeRate(string value)
        {
            var splitted = value.Split(",");
            return new ExchangeRate(Convert.ToDecimal(splitted[0]), splitted[1], splitted[2], splitted[3]);
        }
    }
}