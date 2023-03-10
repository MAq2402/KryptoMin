using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KryptoMin.Application.Contracts;
using System.IO;
using Newtonsoft.Json;
using KryptoMin.Application.Dtos;
using System.Net;

namespace KryptoMin.Function
{
    public class GenerateReport
    {
        private readonly IReportService _service;

        public GenerateReport(IReportService service)
        {
            _service = service;
        }

        [FunctionName(nameof(GenerateReport))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<GenerateRequestDto>
                (await new StreamReader(req.Body).ReadToEndAsync());
            var result = await _service.Generate(request);

            log.LogInformation("C# HTTP trigger function processed a request.");
            if (result.IsFailure)
            {
                log.LogError(result.Error);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return new OkObjectResult(result.Value);
        }
    }
}
