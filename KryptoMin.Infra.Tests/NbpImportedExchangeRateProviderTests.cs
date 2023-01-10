using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Infra.Services;
using Moq;
using Xunit;
using System.Linq;

namespace KryptoMin.Infra.Tests
{
    public class NbpImportedExchangeRateProviderTests
    {
        [Fact]
        public async Task Get_ShouldWork()
        {
            var repo = new Mock<IExchangeRatesRepository>();
            var sut = new NbpImportedExchangeRatesProvider(repo.Object);
            var request = new List<ExchangeRateRequestDto>()
            {
                new ExchangeRateRequestDto("USD", DateTime.Parse("2022-10-05")),
                new ExchangeRateRequestDto("USD", DateTime.Parse("2022-10-05")),
                new ExchangeRateRequestDto("USD", DateTime.Parse("2022-10-06")),
                new ExchangeRateRequestDto("GBP", DateTime.Parse("2022-10-07")),
                new ExchangeRateRequestDto("GBP", DateTime.Parse("2022-10-07")),
                new ExchangeRateRequestDto("GBP", DateTime.Parse("2022-10-07")),

            };
            await sut.Get(request);

            repo.Verify(x => x.GetExchangeRates(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Count() == 3)), Times.Once);
            repo.Verify(x => x.GetExchangeRates(It.Is<IEnumerable<ExchangeRateRequestDto>>(x =>
                x.Any(x => x.Currency == "USD" && x.Date == DateTime.Parse("2022-10-05")))), Times.Once);
            repo.Verify(x => x.GetExchangeRates(It.Is<IEnumerable<ExchangeRateRequestDto>>(x =>
                x.Any(x => x.Currency == "USD" && x.Date == DateTime.Parse("2022-10-06")))), Times.Once);
            repo.Verify(x => x.GetExchangeRates(It.Is<IEnumerable<ExchangeRateRequestDto>>(x =>
                x.Any(x => x.Currency == "GBP" && x.Date == DateTime.Parse("2022-10-07")))), Times.Once);
        }
    }
}