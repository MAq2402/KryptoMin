using System.Globalization;

namespace KryptoMin.Domain.ValueObjects
{
    public class ExchangeRate : ValueObject
    {
        private const string DateFormat = "yyyy-MM-dd";
        public static readonly string DefaultCurrency = "PLN";
        public ExchangeRate(decimal value, string number, DateTime date, string currency)
        {
            Value = value;
            Number = number;
            Date = date.Date;
            Currency = currency;
        }

        public decimal Value { get;  }
        public string Number { get; }
        public DateTime Date { get; }
        public string Currency { get; }

        public static ExchangeRate Default => new ExchangeRate(1, string.Empty, DateTime.Now, DefaultCurrency);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Date;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Number},{Date.ToString(DateFormat)},{Currency}";
        }

        public static explicit operator ExchangeRate(string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            var splitted = value.Split(",");

            return new ExchangeRate(ParseValue(splitted[0]), splitted[1], DateTime.ParseExact(splitted[2], DateFormat, CultureInfo.InvariantCulture), splitted[3]);
        }

        private static decimal ParseValue(string value)
        {
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            return decimal.Parse(value, numberFormatInfo);
        }
    }
}