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

        public async Task Send(SendReportRequestDto request)
        {
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));
            try
            {
                await _emailSender.Send(request.Email, report);
                report.Succeed(request.Email);
                await _reportRepository.Update(report);

            }
            catch (Exception)
            {
                report.Fail(request.Email);
                await _reportRepository.Update(report);
                throw;
            }
        }
    }
}