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
    public class ReportReceiver
    {
        private IReportService _reportService;

        public ReportReceiver(IReportService reportService)
        {
            _reportService = reportService;
        }

        [FunctionName("ReportReceiver")]
        // accept only get
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<GetReportRequestDto>
                (await new StreamReader(req.Body).ReadToEndAsync());

            return new OkObjectResult(await _reportService.Get(request));
        }
    }
}
