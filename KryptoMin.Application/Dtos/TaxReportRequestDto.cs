using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Dtos
{
    public class TaxReportRequestDto
    {
        public IEnumerable<TransactionDto> Transactions { get; set; }
        public decimal PreviousYearLoss { get; set; }
    }
}