using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Settings;
using KryptoMin.Infra.Abstract;
using KryptoMin.Infra.Models.AzureTableStorage;
using Microsoft.Azure.Cosmos.Table;

namespace KryptoMin.Infra.Services
{
    public class ExchangeRatesRepository : AzureTableStorageRepository, IExchangeRatesRepository
    {
        private readonly CloudTable _exchangeRates;

        public ExchangeRatesRepository(DbSettings dbSettings) : base(dbSettings)
        {
            _exchangeRates = CreateCloudTableClient("ExchangeRates");
        }

        public async Task Insert(IEnumerable<NbpCsvExchnageRateDto> xd)
        {
            var batchTransactionInsert = new TableBatchOperation();
            var partitionKey = Guid.NewGuid();

            foreach (var item in xd)
            {
                var insertReport = TableOperation.Insert((new ExchangeRateTableEntity
                {
                    Data = item.Data,
                    THB = item.THB,
                    USD = item.USD,
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = partitionKey.ToString()
                }));
                await _exchangeRates.ExecuteAsync(insertReport);
            }
        }

        public async Task RemoveAll()
        {
            var items = _exchangeRates.ExecuteQuery(new TableQuery<ExchangeRateTableEntity>());
            foreach (var item in items)
            {
                await _exchangeRates.ExecuteAsync(TableOperation.Delete(item));
            }
        }
    }
}