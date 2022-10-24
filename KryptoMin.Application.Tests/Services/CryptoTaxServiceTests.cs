using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Services;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Enums;
using KryptoMin.Domain.Repositories;
using KryptoMin.Domain.Services;
using KryptoMin.Domain.ValueObjects;
using Moq;
using Xunit;

namespace KryptoMin.Application.Tests.Services
{
    public class CryptoTaxServiceTests
    {
        [Fact]
        public async Task GenerateReport_ShouldWork()
        {
            var exchangeRateProvider = new Mock<IExchangeRateProvider>();
            var reportRepository = new Mock<IRepository<TaxReport>>();
            var taxReportCalculator = new Mock<ITaxReportCalculator>();
            var exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate(1, "1", "2022-05-17", "PLN"),
                new ExchangeRate(2.53m, "2", "2022-05-16", "EUR"),
                new ExchangeRate(4.21m, "3", "2022-05-16", "USD"),
                new ExchangeRate(6.97m, "4", "2022-05-13", "USD"),
                new ExchangeRate(7.80m, "5", "2022-05-13", "EUR"),
            };

            var taxReport = new TaxReport(Guid.NewGuid(), Guid.NewGuid(), new List<Transaction>(), 0, 0, 0, 0, "email@mail.com", TaxReportStatus.Created);
            taxReportCalculator.Setup(x =>
                x.Calculate(It.IsAny<List<Transaction>>(), It.IsAny<List<ExchangeRate>>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(taxReport);
            exchangeRateProvider.Setup(x => x.Get(It.IsAny<IEnumerable<ExchangeRateRequestDto>>())).ReturnsAsync(exchangeRates);
            var sut = new CryptoTaxService(exchangeRateProvider.Object, reportRepository.Object, taxReportCalculator.Object);

            var taxReportRequest = new TaxReportRequestDto
            {
                PreviousYearLoss = 999m,
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-18"),
                        Method = "Credit Card",
                        Amount = "941.54 PLN",
                        Price = "4.61356493 USDT/PLN",
                        Fees = "18.83 PLN",
                        FinalAmount = "200 USDT",
                        Status = "Completed",
                        IsSell = false,
                        TransactionId = "N01223522377463013376051811"
                    },
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-17"),
                        Method = "Credit Card",
                        Amount = "500.54 EUR",
                        Price = "4.61356493 USDT/PLN",
                        Fees = "10.83 USD",
                        FinalAmount = "200 USDT",
                        Status = "Completed",
                        IsSell = false,
                        TransactionId = "N01223522377463013376051812"
                    },
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-16"),
                        Method = "Credit Card",
                        Amount = "2000.99 USD",
                        Price = "4.61356493 USDT/PLN",
                        Fees = string.Empty,
                        FinalAmount = "200 USDT",
                        Status = "Completed",
                        IsSell = true,
                        TransactionId = "N0122352237746301337605183"
                    }
                }
            };

            var actual = await sut.GenerateReport(taxReportRequest);
            var expected = new TaxReportResponseDto
            {
                PartitionKey = taxReport.PartitionKey.ToString(),
                RowKey = taxReport.RowKey.ToString()
            };

            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "PLN" && x.Date == "2022-05-17"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-16"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-16"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-13"))), Times.Once);
            taxReportCalculator.Verify(x => x.Calculate(It.IsAny<List<Transaction>>(), exchangeRates, It.IsAny<Guid>(), 999m), Times.Once);
            reportRepository.Verify(x => x.Add(taxReport), Times.Once);

            actual.PartitionKey.Should().Be(expected.PartitionKey);
            actual.RowKey.Should().Be(expected.RowKey);
        }
    }
}