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
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public ReportService(IEmailSender emailSender, IRepository<TaxReport> reportRepository, IExchangeRateProvider exchangeRateProvider)
        {
            _reportRepository = reportRepository;
            _emailSender = emailSender;
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<ReportResponseDto> Send(SendReportRequestDto request)
        {
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));

            if (report is null)
            {
                throw new ArgumentNullException("Report with given ids has not been found.");
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
                Tax = report.Tax
            };
        }

        public async Task<GenerateResponseDto> Generate(GenerateRequestDto request)
        {
            var reportId = Guid.NewGuid();
            var transactions = request.Transactions.Select(x =>
                new Transaction(reportId, Guid.NewGuid(), x.Date, new Amount(x.Amount),
                string.IsNullOrEmpty(x.Fees) ? Amount.Zero : new Amount(x.Fees), x.IsSell)
            ).ToList();

            var requestsForAmounts = transactions.Select(x => new ExchangeRateRequestDto(x.Amount.Currency, x.PreviousWorkingDay));
            var requestsForFees = transactions.Where(x => x.HasFees).Select(x => new ExchangeRateRequestDto(x.Fees.Currency, x.PreviousWorkingDay));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var report = TaxReport.Generate(reportId, Guid.NewGuid(), transactions, exchangeRates, request.PreviousYearLoss);

            await _reportRepository.Add(report);
            return new GenerateResponseDto
            {
                PartitionKey = report.PartitionKey.ToString(),
                RowKey = report.RowKey.ToString()
            };
        }
    }
}