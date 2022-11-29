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

namespace KryptoMin.Function
{
    public class CryptoTaxReport
    {
        private readonly IReportService _service;

        public CryptoTaxReport(IReportService service)
        {
            _service = service;
        }

        [FunctionName("CryptoTaxReport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<GenerateRequestDto>
                (await new StreamReader(req.Body).ReadToEndAsync());
            var result = await _service.Generate(request);
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(result);
        }
    }
}
