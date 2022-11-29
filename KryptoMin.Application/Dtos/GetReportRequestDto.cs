namespace KryptoMin.Application.Dtos
{
    public class GetReportRequestDto
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}