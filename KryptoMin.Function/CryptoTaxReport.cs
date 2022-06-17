using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KryptoMin.Application.Contracts;
using System.Collections.Generic;
using KryptoMin.Application.Models;
using System.IO;
using Newtonsoft.Json;

namespace KryptoMin.Function
{
    public class CryptoTaxReport
    {
        private readonly ICryptoTaxService _cryptoTaxService;

        public CryptoTaxReport(ICryptoTaxService cryptoTaxService)
        {
            _cryptoTaxService = cryptoTaxService;
        }

        [FunctionName("CryptoTaxReport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var request = JsonConvert.DeserializeObject<TaxReportRequestDto>
                (await new StreamReader(req.Body).ReadToEndAsync());
            var result = await _cryptoTaxService.GenerateReport(request);
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(result);
        }
    }
}
