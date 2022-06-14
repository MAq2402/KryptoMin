using System.Globalization;

namespace KryptoMin.Application.Models
{
    public class Amount
    {
        public Amount(string amount)
        {
            var splittedAmount = amount.Split(" ");
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            Value = decimal.Parse(splittedAmount[0], numberFormatInfo);
            Currency = splittedAmount[1];
        }

        public decimal Value { get; set; }
        public string Currency { get; set; }
    }
}