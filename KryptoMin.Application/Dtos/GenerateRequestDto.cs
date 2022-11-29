namespace KryptoMin.Application.Dtos
{
    public class GenerateRequestDto
    {
        public IEnumerable<TransactionDto> Transactions { get; set; }
        public decimal PreviousYearLoss { get; set; }
    }
}