using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KryptoMin.Application.Contracts;
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
            var exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate(1, "1", "2022-05-17", "PLN"),
                new ExchangeRate(2.53m, "2", "2022-05-16", "EUR"),
                new ExchangeRate(4.21m, "3", "2022-05-16", "USD"),
                new ExchangeRate(6.97m, "4", "2022-05-15", "USD"),
                new ExchangeRate(7.80m, "5", "2022-05-15", "EUR"),
            };
            currencyProvider.Setup(x => x.Get(It.IsAny<IEnumerable<ExchangeRateRequest>>())).ReturnsAsync(exchangeRates);
            var sut = new CryptoTaxService(currencyProvider.Object);

            var taxReportRequest = new TaxReportRequestDto
            {
                PreviousYearLoss = 999m,
                Transactions = new List<PurchaseDto>
                {
                    new PurchaseDto
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
                    new PurchaseDto
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
                    new PurchaseDto
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
                Purchases = new List<PurchaseResponseDto>
                {
                    new PurchaseResponseDto
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
                    new PurchaseResponseDto
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
                    new PurchaseResponseDto
                    {
                        Amount = new Amount("2000.99 USD"),
                        Costs = 391.638m,
                        Date = DateTime.Parse("2022-05-16"),
                        ExchangeRateAmount = new ExchangeRate(6.97m, "4", "2022-05-15", "USD"),
                        ExchangeRateFees = new ExchangeRate(7.80m, "5", "2022-05-15", "EUR"),
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

            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequest>>(x => x.Any(x => x.Currency == "PLN" && x.Date == "2022-05-17"))));
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequest>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-16"))));
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequest>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-16"))));
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequest>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-15"))));
            currencyProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequest>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-15"))));

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
            actual.Purchases.ToList()[index].Amount.Should().Be(expected.Purchases.ToList()[index].Amount);
            actual.Purchases.ToList()[index].Costs.Should().Be(expected.Purchases.ToList()[index].Costs);
            actual.Purchases.ToList()[index].Date.Should().Be(expected.Purchases.ToList()[index].Date);
            actual.Purchases.ToList()[index].ExchangeRateAmount.Should().Be(expected.Purchases.ToList()[index].ExchangeRateAmount);
            actual.Purchases.ToList()[index].ExchangeRateFees.Should().Be(expected.Purchases.ToList()[index].ExchangeRateFees);
            actual.Purchases.ToList()[index].Fees.Should().Be(expected.Purchases.ToList()[index].Fees);
            actual.Purchases.ToList()[index].FinalAmount.Should().Be(expected.Purchases.ToList()[index].FinalAmount);
            actual.Purchases.ToList()[index].IsSell.Should().Be(expected.Purchases.ToList()[index].IsSell);
            actual.Purchases.ToList()[index].Method.Should().Be(expected.Purchases.ToList()[index].Method);
            actual.Purchases.ToList()[index].Price.Should().Be(expected.Purchases.ToList()[index].Price);
            actual.Purchases.ToList()[index].Profits.Should().Be(expected.Purchases.ToList()[index].Profits);
            actual.Purchases.ToList()[index].TransactionId.Should().Be(expected.Purchases.ToList()[index].TransactionId);
        }
    }
}