using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KryptoMin.Application.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using KryptoMin.Application.Dtos;

namespace KryptoMin.Function
{
    public class SendReport
    {
        private IReportService _reportService;

        public SendReport(IReportService reportService)
        {
            _reportService = reportService;
        }

        [FunctionName(nameof(SendReport))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<SendReportRequestDto>
                (await new StreamReader(req.Body).ReadToEndAsync());

            return new OkObjectResult(await _reportService.Send(request));
        }
    }
}
