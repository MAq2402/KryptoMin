namespace KryptoMin.Application.Models
{
    public class Purchase
    {
        public Purchase(DateTime date, string method, Amount amount, string price, Amount fees, 
            string finalAmount, string transactionId)
        {
            Date = date;
            Method = method;
            Amount = amount;
            Price = price;
            Fees = fees;
            FinalAmount = finalAmount;
            TransactionId = transactionId;
        }

        public DateTime Date { get; }
        public DateTime DayBefore => Date.AddDays(-1);
        public string FormattedDayBefore => DayBefore.ToString("yyyy-MM-dd");
        public string Method { get; }
        public Amount Amount { get; }
        public string Price { get; }
        public Amount Fees { get; }
        public string FinalAmount { get; }
        public string TransactionId { get; }
    }
}