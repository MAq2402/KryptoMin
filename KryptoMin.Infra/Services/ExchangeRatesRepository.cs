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

        public async Task<List<ExchangeRate>> GetExchangeRates(IEnumerable<ExchangeRateRequestDto> requests)
        {
            var exchangeRates = _exchangeRates.ExecuteQuery(new TableQuery<ExchangeRateTableEntity>()).ToList();
            var result = new List<ExchangeRate>();
            foreach (var request in requests)
            {
                var exchangeRate = exchangeRates.Where(x => ParseDate(x.Date) < request.Date).OrderByDescending(x => x.Date).First();
                result.Add(new ExchangeRate(GetValue(exchangeRate, request.Currency), exchangeRate.FullNumber, ParseDate(exchangeRate.Date), request.Currency));
            }

            return await Task.FromResult(result);
        }

        private DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        private decimal GetValue(object src, string propName)
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