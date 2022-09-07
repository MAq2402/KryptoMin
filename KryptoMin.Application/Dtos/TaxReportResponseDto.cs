namespace KryptoMin.Application.Dtos
{
    public class TaxReportResponseDto
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        // public IEnumerable<TransactionResponseDto> Transactions { get; set; }
        // public decimal Balance { get; set; }
        // public decimal BalanceWithPreviousYearLoss { get; set; }
        // public decimal Tax { get; set; }
        // public decimal PreviousYearLoss { get; set; }
    }
}