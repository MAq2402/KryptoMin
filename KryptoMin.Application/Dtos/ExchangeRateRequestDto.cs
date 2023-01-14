namespace KryptoMin.Application.Dtos
{
    public class ExchangeRateRequestDto
    {
        public ExchangeRateRequestDto(string currency, DateTime date)
        {
            Date = date;
            Currency = currency;
        }
        
        public DateTime Date { get; }
        public string Currency { get; }
    }
}