namespace KryptoMin.Application.Dtos
{
    public class TaxReportDto
    {
        public IEnumerable<TransactionResponseDto> Transactions { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithPreviousYearLoss { get; set; }
        public decimal Tax { get; set; }
        public decimal PreviousYearLoss { get; set; }
    }
}