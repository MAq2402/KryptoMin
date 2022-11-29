using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Repositories;

namespace KryptoMin.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<TaxReport> _reportRepository;
        private readonly IEmailSender _emailSender;

        public ReportService(IEmailSender emailSender, IRepository<TaxReport> reportRepository)
        {
            _reportRepository = reportRepository;
            _emailSender = emailSender;
        }

        public async Task<TaxReportResponseDto> Send(SendReportRequestDto request)
        {
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));

            if (report is null)
            {
                throw new ArgumentNullException("Report with given ids has not been found.");
            }
            try
            {
                await _emailSender.Send(request.Email, report);
                report.Succeed(request.Email);
                await _reportRepository.Update(report);
                return MapToResponse(report);
            }
            catch (Exception)
            {
                report.Fail(request.Email);
                await _reportRepository.Update(report);
                throw;
            }
        }

        public async Task<TaxReportResponseDto> Get(GetReportRequestDto request)
        {
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));

            if (report is null)
            {
                throw new ArgumentNullException("Report with given ids has not been found.");
            }

            return MapToResponse(report);
        }

        private TaxReportResponseDto MapToResponse(TaxReport report)
        {
            return new TaxReportResponseDto
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
    }
}