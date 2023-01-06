using System.Globalization;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Settings;
using KryptoMin.Domain.ValueObjects;
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

        public async Task<ExchangeRate> GetForPreviousWorkingDay(string currency, DateTime date)
        {
            //Execute only once - pass collection of currencies ;)
            // Getting currencies from refelction?
            // Setting numbers 
            // Get only once exchange rate for same currency and date pair 
            var items = _exchangeRates.ExecuteQuery(new TableQuery<ExchangeRateTableEntity>()).ToList();

            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
            var result = items.Where(x => ParseDate(x.Date) <= date).OrderByDescending(x => x.Date).First();
            return await Task.FromResult(new ExchangeRate(decimal.Parse(result.USD, numberFormatInfo), "TODO", ParseDate(result.Date), currency));
        }

        private DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public async Task Insert(IEnumerable<NbpCsvExchnageRateDto> items)
        {
            var batchTransactionInsert = new TableBatchOperation();
            var partitionKey = Guid.NewGuid();

            foreach (var item in items)
            {
                var insertReport = TableOperation.Insert((new ExchangeRateTableEntity
                {
                    Date = item.Date,
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