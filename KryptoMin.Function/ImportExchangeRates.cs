using System.IO;
using System.Threading.Tasks;
using KryptoMin.Application.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace KryptoMin.Function
{
    [StorageAccount("StorageConnectionString")]
    public class ImportExchangeRates
    {
        private readonly IExchangeRatesImportService _exchangeRatesImportService;

        public ImportExchangeRates(IExchangeRatesImportService exchangeRatesImportService)
        {
            _exchangeRatesImportService = exchangeRatesImportService;
        }

        [FunctionName(nameof(ImportExchangeRates))]        
        public async Task Run([BlobTrigger("kryptomin-blob-container/{name}")] Stream myBlob, string name, ILogger log)
        {
            await _exchangeRatesImportService.Import(myBlob);
        }
    }
}