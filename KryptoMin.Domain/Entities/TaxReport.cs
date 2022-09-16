using KryptoMin.Domain.Enums;

namespace KryptoMin.Domain.Entities
{
    public class TaxReport : Entity
    {
        private List<Transaction> _transactions;

        public TaxReport(Guid partitionKey,
            Guid rowKey, 
            IEnumerable<Transaction> transactions, 
            decimal balance, 
            decimal balanceWithPreviousYearLoss, 
            decimal tax, 
            decimal previousYearLoss, 
            string ownerEmail, 
            TaxReportStatus status) : base(partitionKey, rowKey)
        {
            _transactions = transactions.ToList();
            Balance = balance;
            BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss;
            Tax = tax;
            PreviousYearLoss = previousYearLoss;
            OwnerEmail = ownerEmail;
            Status = status;
        }

        public IEnumerable<Transaction> Transactions => _transactions;
        public decimal Balance { get; }
        public decimal BalanceWithPreviousYearLoss { get; }
        public decimal Tax { get; }
        public decimal PreviousYearLoss { get; }
        public string OwnerEmail { get; private set; }
        public TaxReportStatus Status { get; private set; }

        public void Succeed(string ownerEmail)
        {
            OwnerEmail = ownerEmail;
            Status = TaxReportStatus.Received;
        }

        public void Fail(string ownerEmail)
        {
            OwnerEmail = ownerEmail;
            Status = TaxReportStatus.FailedToSend;
        }
    }
}