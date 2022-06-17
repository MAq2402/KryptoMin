namespace KryptoMin.Application.Models
{
    public class TaxReportRequestDto
    {
        public IEnumerable<PurchaseDto> Transactions { get; set; }
        public decimal PreviousYearLoss { get; set; }
    }
}