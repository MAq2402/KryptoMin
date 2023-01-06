using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;

namespace KryptoMin.Application.Services
{
    public class ExchangeRatesImportService : IExchangeRatesImportService
    {
        private IExchangeRatesRepository _exchangeRatesRepository;

        public ExchangeRatesImportService(IExchangeRatesRepository exchangeRatesRepository)
        {
            _exchangeRatesRepository = exchangeRatesRepository;
        }

        public async Task Import(Stream stream)
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,
                MissingFieldFound = (x) => { },
            };
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                await _exchangeRatesRepository.RemoveAll();
                var records = csv.GetRecords<NbpCsvExchnageRateDto>().ToList().Skip(2).TakeWhile(item => item.Date.All(x => char.IsDigit(x)));
                await _exchangeRatesRepository.Insert(records);
            }
        }
    }
}