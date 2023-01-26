using CSharpFunctionalExtensions;
using KryptoMin.Domain.Enums;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Domain.Entities
{
    public class TaxReport : Entity
    {
        private List<Transaction> _transactions;
        private const decimal TaxRate = 0.19m;
        private const int DecimalPlacesForTax = 0;

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

            var calculateRevenueResult = CalculateRevenue();
            var calculateCostsResult = CalculateCosts();
            if (calculateRevenueResult.IsFailure || calculateCostsResult.IsFailure)
            {
                GenerationSucceded = Result.Combine(calculateCostsResult, calculateRevenueResult);
            }
            else
            {
                Revenue = calculateRevenueResult.Value;
                Costs = calculateCostsResult.Value;
                Income = Revenue - (Costs + PreviousYearsCosts) > 0 ? Revenue - (Costs + PreviousYearsCosts) : 0;
                CurrentYearCosts = (Costs + PreviousYearsCosts) - Revenue > 0 ? (Costs + PreviousYearsCosts) - Revenue : 0;
                Tax = Income > 0 ? Math.Round(Income * TaxRate, DecimalPlacesForTax) : 0;
                GenerationSucceded = Result.Success();
            }
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

        public decimal Revenue { get; }
        public decimal Costs { get; }
        public decimal PreviousYearsCosts { get; }
        public decimal Income { get; }
        public decimal CurrentYearCosts { get; }
        public decimal Tax { get; }

        public IEnumerable<Transaction> Transactions => _transactions;
        public string OwnerEmail { get; private set; }
        public TaxReportStatus Status { get; private set; }
        public Result GenerationSucceded { get; }

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

        private Result<decimal> CalculateRevenue()
        {
            return SumTransactionsResults(x => x.CalculateProfits());
        }

        private Result<decimal> CalculateCosts()
        {
            return SumTransactionsResults(x => x.CalculateCosts());
        }

        private Result<decimal> SumTransactionsResults(Func<Transaction, Result<decimal>> calculationFunc)
        {
            var sum = 0.00m;
            foreach (var transaction in _transactions)
            {
                var calculateProfitsResult = calculationFunc(transaction);
                if (calculateProfitsResult.IsSuccess)
                {
                    sum += calculateProfitsResult.Value;
                }
                else
                {
                    return calculateProfitsResult;
                }
            }

            return Result.Success(sum);
        }
    }
}