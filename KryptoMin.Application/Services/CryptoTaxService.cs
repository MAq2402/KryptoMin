using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Repositories;
using KryptoMin.Domain.Services;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Application.Services
{
    public class CryptoTaxService : ICryptoTaxService
    {
        private const decimal TaxRate = 0.19m;
        private const int DecimalPlaces = 2;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly IRepository<TaxReport> _reportRepository;
        private readonly ITaxReportCalculator _taxReportCalculator;

        public CryptoTaxService(IExchangeRateProvider exchangeRateProvider, IRepository<TaxReport> reportRepository, ITaxReportCalculator taxReportCalculator)
        {
            _exchangeRateProvider = exchangeRateProvider;
            _reportRepository = reportRepository;
            _taxReportCalculator = taxReportCalculator;
        }

        public async Task<TaxReportResponseDto> GenerateReport(TaxReportRequestDto request)
        {
            var reportId = Guid.NewGuid();
            var transactions = request.Transactions.Select(x =>
                new Transaction(reportId, Guid.NewGuid(), x.Date, x.Method, new Amount(x.Amount), 
                x.Price, string.IsNullOrEmpty(x.Fees) ? null : new Amount(x.Fees), x.FinalAmount, x.IsSell, x.TransactionId)
            ).ToList();

            var requestsForAmounts = transactions.Select(x => new ExchangeRateRequestDto(x.Amount.Currency, x.FormattedPreviousWorkingDay));
            var requestsForFees = transactions.Where(x => x.Fees != null).Select(x => new ExchangeRateRequestDto(x.Fees.Currency, x.FormattedPreviousWorkingDay));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var report = _taxReportCalculator.Calculate(transactions, exchangeRates, reportId, request.PreviousYearLoss);

            await _reportRepository.Add(report);
            return new TaxReportResponseDto
            {
                PartitionKey = report.PartitionKey.ToString(),
                RowKey = report.RowKey.ToString()
            };
        }
    }
}