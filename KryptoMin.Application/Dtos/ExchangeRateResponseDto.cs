namespace KryptoMin.Application.Dtos
{
    public class ExchangeRateResponseDto
    {
        public decimal Value { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
    }
}