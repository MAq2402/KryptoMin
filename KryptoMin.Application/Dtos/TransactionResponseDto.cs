using KryptoMin.Application.Models;

namespace KryptoMin.Application.Dtos
{
    public class TransactionResponseDto
    {
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public Amount Amount { get; set; }
        public string Price { get; set; }
        public Amount Fees { get; set; }
        public string FinalAmount { get; set; }
        public string TransactionId { get; set; }
        public ExchangeRate ExchangeRateFees { get; set; }
        public ExchangeRate ExchangeRateAmount { get; set; }
        public bool IsSell { get; set; }
        public decimal Costs { get; set; }
        public decimal Profits { get; set; }
    }
}