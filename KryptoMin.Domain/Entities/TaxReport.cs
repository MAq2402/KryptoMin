using KryptoMin.Domain.Enums;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Entities
{
    public class TaxReport : Entity
    {
        private List<Transaction> _transactions;
        private const decimal TaxRate = 0.19m;

        public TaxReport(Guid partitionKey,
            Guid rowKey, 
            IEnumerable<Transaction> transactions,
            decimal previousYearCosts, 
            string ownerEmail, 
            TaxReportStatus status) : base(partitionKey, rowKey)
        {
            _transactions = transactions.ToList();
            PreviousYearsCosts = previousYearCosts;
            OwnerEmail = ownerEmail;
            Status = status;
        }

        private TaxReport(Guid partitionKey,
            Guid rowKey, 
            IEnumerable<Transaction> transactions,
            IEnumerable<ExchangeRate> exchangeRates,
            decimal previousYearCosts,
            TaxReportStatus status = TaxReportStatus.Created) : base(partitionKey, rowKey)
        {
            _transactions = transactions.ToList();
            _transactions.ForEach(x => x.AssignExchangeRates(exchangeRates));
            PreviousYearsCosts = previousYearCosts;
            Status = status;
        }

        public static TaxReport Generate(Guid partitionKey,
            Guid rowKey, 
            IEnumerable<Transaction> transactions,
            IEnumerable<ExchangeRate> exchangeRates,
            decimal previousYearCosts,
            TaxReportStatus status = TaxReportStatus.Created)
        {
            return new TaxReport(partitionKey, rowKey, transactions, exchangeRates, previousYearCosts, status);
        }

        public decimal Revenue => _transactions.Sum(x => x.CalculateProfits());
        public decimal Costs => _transactions.Sum(x => x.CalculateCosts());
        public decimal PreviousYearsCosts { get; }
        public decimal Income => Revenue - (Costs + PreviousYearsCosts) > 0 ? Revenue - (Costs + PreviousYearsCosts) : 0;
        public decimal CurrentYearCosts => (Costs + PreviousYearsCosts) - Revenue > 0 ? (Costs + PreviousYearsCosts) - Revenue : 0;
        public decimal Tax => Income > 0 ? Math.Round(Income * TaxRate, 0) : 0;

        public IEnumerable<Transaction> Transactions => _transactions;
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