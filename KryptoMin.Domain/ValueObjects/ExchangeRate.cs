using System.Globalization;

namespace KryptoMin.Domain.ValueObjects
{
    public class ExchangeRate : ValueObject
    {
        public ExchangeRate(decimal value, string number, DateTime date, string currency)
        {
            Value = value;
            Number = number;
            Date = date;
            Currency = currency;
        }

        public ExchangeRate(decimal value, string number, string date, string currency)
        {
            Value = value;
            Number = number;
            Date = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Currency = currency;
        }

        public decimal Value { get;  }
        public string Number { get; }
        public DateTime Date { get; }
        public string FormattedDate => Date.ToString("yyyy-MM-dd");
        public string Currency { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return FormattedDate;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Number},{FormattedDate},{Currency}";
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