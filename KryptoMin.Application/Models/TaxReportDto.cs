namespace KryptoMin.Application.Models
{
    public class TaxReportDto
    {
        public IEnumerable<PurchaseResponseDto> Purchases { get; set; }
        public IEnumerable<object> Sells { get; set; }
        public decimal Balance { get; set; }
        public decimal Tax { get; set; }
    }
}