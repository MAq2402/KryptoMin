namespace KryptoMin.Application.Models
{
    //CONVERT TO "CARD TYPE", Purchase => Transatction and add type Buy/Sell
    // FRONT HANDLE IT? 
    public class SellDto
    {
        public DateTime Date { get; set; }
        public string Coin { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string IndicatedAmount { get; set; }
        public string Fees { get; set; }
        public string OrderId { get; set; }
        //Day before to value object and reuse it.
        public string FormattedDayBefore => Date.AddDays(-1).ToString("yyyy-MM-dd");
    }
}