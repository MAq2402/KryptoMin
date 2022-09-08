namespace KryptoMin.Application.Dtos
{
    public class SendReportRequestDto
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Email { get; set; }
    }
}