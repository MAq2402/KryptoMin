namespace KryptoMin.Application.Dtos
{
    public class ExchangeRateResponseDto
    {
        public decimal Value { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
    }
    
    public class ReportResponseDto
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public decimal Revenue { get; set; }
        public decimal Costs { get; set; }
        public decimal PreviousYearsCosts { get; set; }
        public decimal Income { get; set; }
        public decimal CurrentYearCosts { get; set; }
        public decimal Tax { get; set; }
        public IEnumerable<ExchangeRateResponseDto> ExchangeRates { get; set; }
    }
}