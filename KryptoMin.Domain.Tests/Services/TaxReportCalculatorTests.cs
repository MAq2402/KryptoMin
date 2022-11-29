using System;
using System.Collections.Generic;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Domain.Tests.Services
{
    public class TaxReportCalculatorTests
    {
        [Fact]
        public void Calculate_ShouldWork()
        {
            var partitionKey = Guid.NewGuid();
            var exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate(1, "1", DateTime.Parse("2022-05-17"), "PLN"),
                new ExchangeRate(2.53m, "2", DateTime.Parse("2022-05-16"), "EUR"),
                new ExchangeRate(4.21m, "3", DateTime.Parse("2022-05-16"), "USD"),
                new ExchangeRate(6.97m, "4", DateTime.Parse("2022-05-13"), "USD"),
                new ExchangeRate(7.80m, "5", DateTime.Parse("2022-05-13"), "EUR"),
            };
            var transactions = new List<Transaction>
            {
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-18"),
                    "Credit Card", new Amount("941.54 PLN"), "4.61356493 USDT/PLN",
                    new Amount("18.83 PLN"), "200 USDT", false, "N01223522377463013376051811"),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-17"),
                    "Credit Card", new Amount("500.54 EUR"), "4.61356493 USDT/PLN",
                    new Amount("10.83 USD"), "200 USDT", false, "N01223522377463013376051812"),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-16"),
                    "Credit Card", new Amount("2000.99 USD"), "4.61356493 USDT/PLN",
                    new Amount("50.21 EUR"), "200 USDT", true, "N0122352237746301337605183"),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-18"),
                    "Credit Card", new Amount("1.00 PLN"), "4.61356493 USDT/PLN",
                    Amount.Zero, "200 USDT", true, "N0122352237746301337605184")
            };
            var previousYearLoss = 999m;
            // var sut = new TaxReportCalculator();

            // var report = sut.Calculate(transactions, exchangeRates, partitionKey, previousYearLoss);

            // var expectedPreviousYearLoss = 999m;
            // var expectedBalance = 11283.93m;
            // var expectedBalanceWithPreviousYearLoss = 10284.93m;
            // var expectedTax = 1954;
            // var expectedProfits1 = 0.0m;
            // var expoectedCosts1 = 960.37m;
            // var expectedProfits2 = 0.0m;
            // var expectedCosts2 = 1311.96m;
            // var expectedProfits3 = 13946.90m;
            // var expectedCosts3 = 391.64m;
            // var expectedProfits4 = 1;
            // var expectedCosts4 = 0;
            // report.PreviousYearsCosts.Should().Be(expectedPreviousYearLoss);
            // (report.Income + report.PreviousYearsCosts).Should().Be(expectedBalance);
            // report.Income.Should().Be(expectedBalanceWithPreviousYearLoss);
            // report.Tax.Should().Be(expectedTax);
            // report.Transactions.ToList()[0].CalculateProfits().Should().Be(expectedProfits1);
            // report.Transactions.ToList()[0].CalculateCosts().Should().Be(expoectedCosts1);
            // report.Transactions.ToList()[1].CalculateProfits().Should().Be(expectedProfits2);
            // report.Transactions.ToList()[1].CalculateCosts().Should().Be(expectedCosts2);
            // report.Transactions.ToList()[2].CalculateProfits().Should().Be(expectedProfits3);
            // report.Transactions.ToList()[2].CalculateCosts().Should().Be(expectedCosts3);
            // report.Transactions.ToList()[3].CalculateProfits().Should().Be(expectedProfits4);
            // report.Transactions.ToList()[3].CalculateCosts().Should().Be(expectedCosts4);
        }
    }
}