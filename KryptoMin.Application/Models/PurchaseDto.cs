namespace KryptoMin.Application.Models
{
    public class PurchaseDto
    {
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Amount { get; set; }
        public string Price { get; set; }
        public string Fees { get; set; }
        public string FinalAmount { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public bool IsSell { get; set; }
    }
}