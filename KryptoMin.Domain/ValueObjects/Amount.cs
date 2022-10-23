using System.Globalization;

namespace KryptoMin.Domain.ValueObjects
{
    public class Amount : ValueObject
    {
        public Amount(string amount)
        {
            var splittedAmount = amount.Split(" ");
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Value = decimal.Parse(splittedAmount[0], numberFormatInfo);
            Currency = splittedAmount[1];
        }

        private Amount()
        {
            Value = 0;
            Currency = string.Empty;
        }

        public static Amount Zero => new Amount();

        public decimal Value { get; }
        public string Currency { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} {Currency}";
        }
    }
}