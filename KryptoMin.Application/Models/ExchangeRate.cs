namespace KryptoMin.Application.Models
{
    public class ExchangeRate
    {
        public ExchangeRate(double value, string number)
        {
            Value = value;
            Number = number;
        }

        public double Value { get;  }
        public string Number { get; }
    }
}