using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using KryptoMin.Application.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using KryptoMin.Domain.Repositories;
using KryptoMin.Domain.Entities;

namespace KryptoMin.Function
{
    public class Request
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Email { get; set; }
    }
    public class ReportSender
    {
        private readonly IEmailSender _emailSender;
        private readonly IRepository<TaxReport> _reportRepository;

        public ReportSender(IEmailSender emailSender, IRepository<TaxReport> reportRepository)
        {
            _emailSender = emailSender;
            _reportRepository = reportRepository;
        }

        [FunctionName("ReportSender")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<Request>
                (await new StreamReader(req.Body).ReadToEndAsync());
            var report = await _reportRepository.Get(new Guid(request.PartitionKey), new Guid(request.RowKey));
            try
            {
                await _emailSender.Send(request.Email, report);
                report.Succeed(request.Email);
                await _reportRepository.Update(report);

            }
            catch (Exception ex)
            {
                report.Fail(request.Email);
                await _reportRepository.Update(report);
                throw;
            }

            return new OkObjectResult(report);
        }
    }
}
