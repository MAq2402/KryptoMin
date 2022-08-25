using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Models;
using KryptoMin.Application.Services;
using Moq;
using Xunit;

namespace KryptoMin.Application.Tests.Services
{
    public class CryptoTaxServiceTests
    {
        [Fact]
        public async Task GenerateReport_ShouldWork()
        {
            var currencyProvider = new Mock<IExchangeRateProvider>();
            var reportRepository = new Mock<IReportRepository>();
            var exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate(1, "1", "2022-05-17", "PLN"),
                new ExchangeRate(2.53m, "2", "2022-05-16", "EUR"),
                new ExchangeRate(4.21m, "3", "2022-05-16", "USD"),
                new ExchangeRate(6.97m, "4", "2022-05-13", "USD"),
                new ExchangeRate(7.80m, "5", "2022-05-13", "EUR"),
            };
            currencyProvider.Setup(x => x.Get(It.IsAny<IEnumerable<ExchangeRateRequestDto>>())).ReturnsAsync(exchangeRates);
            var sut = new CryptoTaxService(currencyProvider.Object, reportRepository.Object);

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
                        Fees = "50.21 EUR",
                        FinalAmount = "200 USDT",
                        Status = "Completed",
                        IsSell = true,
                        TransactionId = "N0122352237746301337605183"
                    }
                }
            };
            var actual = await sut.GenerateReport(taxReportRequest);
            var expected = new TaxReportDto
            {
                Transactions = new List<TransactionResponseDto>
                {
                    new TransactionResponseDto
                    {
                        Amount = new Amount("941.54 PLN"),
                        Costs = 960.37m,
                        Date = DateTime.Parse("2022-05-18"),
                        ExchangeRateAmount = new ExchangeRate(1, "1", "2022-05-17", "PLN"),
                        ExchangeRateFees = new ExchangeRate(1, "1", "2022-05-17", "PLN"),
                        Fees = new Amount("18.83 PLN"),
                        FinalAmount = "200 USDT",
                        IsSell = false,
                        Method = "Credit Card",
                        Price = "4.61356493 USDT/PLN",
                        Profits = 0.0m,
                        TransactionId = "N01223522377463013376051811"
                    },
                    new TransactionResponseDto
                    {
                        Amount = new Amount("500.54 EUR"),
                        Costs = 1311.9605m,
                        Date = DateTime.Parse("2022-05-17"),
                        ExchangeRateAmount = new ExchangeRate(2.53m, "2", "2022-05-16", "EUR"),
                        ExchangeRateFees =  new ExchangeRate(4.21m, "3", "2022-05-16", "USD"),
                        Fees = new Amount("10.83 USD"),
                        FinalAmount = "200 USDT",
                        IsSell = false,
                        Method = "Credit Card",
                        Price = "4.61356493 USDT/PLN",
                        Profits = 0.0m,
                        TransactionId = "N01223522377463013376051812"
                    },
                    new TransactionResponseDto
                    {
                        Amount = new Amount("2000.99 USD"),
                        Costs = 391.638m,
                        Date = DateTime.Parse("2022-05-16"),
                        ExchangeRateAmount = new ExchangeRate(6.97m, "4", "2022-05-13", "USD"),
                        ExchangeRateFees = new ExchangeRate(7.80m, "5", "2022-05-13", "EUR"),
                        Fees = new Amount("50.21 EUR"),
                        FinalAmount = "200 USDT",
                        IsSell = true,
                        Method = "Credit Card",
                        Price = "4.61356493 USDT/PLN",
                        Profits = 13946.9003m,
                        TransactionId = "N0122352237746301337605183"
                    }
                },
                PreviousYearLoss = 999m,
                Balance = 11282.93m,
                BalanceWithPreviousYearLoss = 10283.93m,
                Tax = 1953.95m
            };

            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "PLN" && x.Date == "2022-05-17"))), Times.Once);
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-16"))), Times.Once);
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-16"))), Times.Once);
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-13"))), Times.Once);
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-13"))), Times.Once);
            reportRepository.Verify(x => x.Save(It.IsAny<TaxReportDto>()), Times.Once);

            CompareTransactions(actual, expected, 0);
            CompareTransactions(actual, expected, 1);
            CompareTransactions(actual, expected, 2);

            actual.PreviousYearLoss.Should().Be(expected.PreviousYearLoss);
            actual.Balance.Should().Be(expected.Balance);
            actual.BalanceWithPreviousYearLoss.Should().Be(expected.BalanceWithPreviousYearLoss);
            actual.Tax.Should().Be(expected.Tax);
        }

        private static void CompareTransactions(TaxReportDto actual, TaxReportDto expected, int index)
        {
            actual.Transactions.ToList()[index].Amount.Should().Be(expected.Transactions.ToList()[index].Amount);
            actual.Transactions.ToList()[index].Costs.Should().Be(expected.Transactions.ToList()[index].Costs);
            actual.Transactions.ToList()[index].Date.Should().Be(expected.Transactions.ToList()[index].Date);
            actual.Transactions.ToList()[index].ExchangeRateAmount.Should().Be(expected.Transactions.ToList()[index].ExchangeRateAmount);
            actual.Transactions.ToList()[index].ExchangeRateFees.Should().Be(expected.Transactions.ToList()[index].ExchangeRateFees);
            actual.Transactions.ToList()[index].Fees.Should().Be(expected.Transactions.ToList()[index].Fees);
            actual.Transactions.ToList()[index].FinalAmount.Should().Be(expected.Transactions.ToList()[index].FinalAmount);
            actual.Transactions.ToList()[index].IsSell.Should().Be(expected.Transactions.ToList()[index].IsSell);
            actual.Transactions.ToList()[index].Method.Should().Be(expected.Transactions.ToList()[index].Method);
            actual.Transactions.ToList()[index].Price.Should().Be(expected.Transactions.ToList()[index].Price);
            actual.Transactions.ToList()[index].Profits.Should().Be(expected.Transactions.ToList()[index].Profits);
            actual.Transactions.ToList()[index].TransactionId.Should().Be(expected.Transactions.ToList()[index].TransactionId);
        }
    }
}