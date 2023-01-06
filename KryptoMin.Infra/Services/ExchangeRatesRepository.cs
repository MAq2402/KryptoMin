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
            // Execute only once - pass collection of currencies ;)
            // Getting currencies from refelction?
            // Setting numbers 
            // Get only once exchange rate for same currency and date pair 
            var items = _exchangeRates.ExecuteQuery(new TableQuery<ExchangeRateTableEntity>()).ToList();

            
            var result = items.Where(x => ParseDate(x.Date) <= date).OrderByDescending(x => x.Date).First();
            return await Task.FromResult(new ExchangeRate(GetValue(result, currency), result.FullNumber, ParseDate(result.Date), currency));
        }

        private DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public decimal GetValue(object src, string propName)
        {
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
            return decimal.Parse(src.GetType().GetProperty(propName).GetValue(src, null) as string, numberFormatInfo);
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
                    AUD = item.AUD,
                    BGN = item.BGN,
                    BRL = item.BRL,
                    CAD = item.CAD,
                    CHF = item.CHF,
                    CLP = item.CLP,
                    CNY = item.CNY,
                    CZK = item.CZK,
                    DKK = item.DKK,
                    EUR = item.EUR,
                    GBP = item.GBP,
                    HKD = item.HKD,
                    HRK = item.HRK,
                    HUF = item.HUF,
                    IDR = item.IDR,
                    ILS = item.ILS,
                    INR = item.INR,
                    ISK = item.ISK,
                    JPY = item.JPY,
                    KRW = item.KRW,
                    MXN = item.MXN,
                    MYR = item.MYR,
                    NOK = item.NOK,
                    Number = item.Number,
                    FullNumber = item.FullNumber,
                    NZD = item.NZD,
                    PHP = item.PHP,
                    RON = item.RON,
                    RUB = item.RUB,
                    SEK = item.SEK,
                    SGD = item.SGD,
                    TRY = item.TRY,
                    UAH = item.UAH,
                    XDR = item.XDR,
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