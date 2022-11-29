namespace KryptoMin.Application.Dtos
{
    public class TaxReportResponseDto
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public decimal Revenue { get; set; } 
        public decimal Costs { get; set; } 
        public decimal PreviousYearsCosts { get; set; } 
        public decimal Income { get; set; } 
        public decimal CurrentYearCosts { get; set; } 
        public decimal Tax { get; set; } 
    }
}