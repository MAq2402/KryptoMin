namespace KryptoMin.Application.Dtos
{
    public class TransactionDto
    {
        public DateTime Date { get; set; }
        public string Amount { get; set; }
        public string Fees { get; set; }
        public bool IsSell { get; set; }
    }
}