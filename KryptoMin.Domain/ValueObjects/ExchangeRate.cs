using System.Globalization;

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
            return $"{Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Number},{Date},{Currency}";
        }

        public static explicit operator ExchangeRate(string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            var splitted = value.Split(",");
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            return new ExchangeRate(decimal.Parse(splitted[0], numberFormatInfo), splitted[1], splitted[2], splitted[3]);
        }
    }
}