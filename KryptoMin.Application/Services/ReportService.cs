using CSharpFunctionalExtensions;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Repositories;
using KryptoMin.Domain.ValueObjects;

namespace KryptoMin.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<TaxReport> _reportRepository;
        private readonly IEmailSender _emailSender;
        private readonly IExchangeRatesProvider _exchangeRateProvider;

        public ReportService(IEmailSender emailSender, IRepository<TaxReport> reportRepository, IExchangeRatesProvider exchangeRateProvider)
        {
            _reportRepository = reportRepository;
            _emailSender = emailSender;
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<Result<ReportResponseDto>> Send(SendReportRequestDto request)
        {
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));

            if (report is null)
            {
                return Result.Failure<ReportResponseDto>("Report with given ids has not been found.");
            }

            try
            {
                await _emailSender.Send(request.Email, report);
            }
            catch (Exception)
            {
                report.Fail(request.Email);
                return await UpdateReport(request, report);
            }
            report.Succeed(request.Email);
            return await UpdateReport(request, report);
        }

        private async Task<ReportResponseDto> UpdateReport(SendReportRequestDto request, TaxReport report)
        {
            await _reportRepository.Update(report);
            return MapToResponse(report);
        }

        private ReportResponseDto MapToResponse(TaxReport report)
        {
            return new ReportResponseDto
            {
                PartitionKey = report.PartitionKey.ToString(),
                RowKey = report.RowKey.ToString(),
                Costs = report.Costs,
                CurrentYearCosts = report.CurrentYearCosts,
                Income = report.Income,
                PreviousYearsCosts = report.PreviousYearsCosts,
                Revenue = report.Revenue,
                Tax = report.Tax,
                ExchangeRates = report.Transactions.Select(x => MapToResponse(x.ExchangeRateForAmount))
                                                    .Concat(report.Transactions
                                                    .Where(x => x.HasFees)
                                                    .Select(x => MapToResponse(x.ExchangeRateForFees)))
                                                    .Where(x => x.Currency != ExchangeRate.DefaultCurrency)
                                                    .DistinctBy(x => new { x.Currency, x.Number })
            };
        }

        private ExchangeRateResponseDto MapToResponse(ExchangeRate exchangeRate)
        {
            return new ExchangeRateResponseDto
            {
                Currency = exchangeRate.Currency,
                Date = exchangeRate.Date,
                Number = exchangeRate.Number,
                Value = exchangeRate.Value
            };
        }

        public async Task<Result<GenerateResponseDto>> Generate(GenerateRequestDto request)
        {
            var reportId = Guid.NewGuid();
            var transactions = request.Transactions.Select(x =>
                new Transaction(reportId, Guid.NewGuid(), x.Date, new Amount(x.Amount),
                string.IsNullOrEmpty(x.Fees) ? Amount.Zero : new Amount(x.Fees), x.IsSell)
            ).ToList();

            var requestsForAmounts = transactions.Select(x => new ExchangeRateRequestDto(x.Amount.Currency, x.Date));
            var requestsForFees = transactions.Where(x => x.HasFees).Select(x => new ExchangeRateRequestDto(x.Fees.Currency, x.Date));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var report = TaxReport.Generate(reportId, Guid.NewGuid(), transactions, exchangeRates, request.PreviousYearLoss);

            if (report.GenerationSucceded.IsFailure)
            {
                return Result.Failure<GenerateResponseDto>(report.GenerationSucceded.Error);
            }
            else
            {
                await _reportRepository.Add(report);
                return new GenerateResponseDto
                {
                    PartitionKey = report.PartitionKey.ToString(),
                    RowKey = report.RowKey.ToString()
                };
            }
        }
    }
}