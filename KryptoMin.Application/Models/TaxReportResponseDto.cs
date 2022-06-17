namespace KryptoMin.Application.Models
{
    public class TaxReportDto
    {
        public IEnumerable<PurchaseResponseDto> Purchases { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceWithPreviousYearLoss { get; set; }
        public decimal Tax { get; set; }
        public decimal PreviousYearLoss { get; set; }
    }
}