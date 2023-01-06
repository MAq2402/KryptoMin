using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Models.AzureTableStorage
{
    public class ExchangeRateTableEntity : TableEntity
    {
        public string Date { get; set; }
        public string THB { get; set; }
        public string USD { get; set; }
    }
}