namespace KryptoMin.Application.Dtos
{
    public class ExchangeRateRequestDto
    {
        public ExchangeRateRequestDto(string currency, string date)
        {
            Date = date;
            Currency = currency;
        }
        
        public string Date { get; }
        public string Currency { get; }
    }
}