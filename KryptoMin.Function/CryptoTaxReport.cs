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

namespace KryptoMin.Function
{
    public class CryptoTaxReport
    {
        private readonly ICurrencyProvider _currencyProvider;

        public CryptoTaxReport(ICurrencyProvider currencyProvider)
        {
            _currencyProvider = currencyProvider;
        }

        [FunctionName("CryptoTaxReport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var purchases = new List<Purchase>
            {
                new Purchase(DateTime.Now.AddDays(-1), "usd")
            };
            var result = await _currencyProvider.Get(purchases);
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(result);
        }
    }
}
