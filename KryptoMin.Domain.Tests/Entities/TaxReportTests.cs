using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Domain.Tests.Entities
{
    public class TaxReportTests
    {
        [Fact]    
        public void Generate_ShouldWork()
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
                    new Amount("941.54 PLN"),
                    new Amount("18.83 PLN"), false),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-17"),
                    new Amount("500.54 EUR"),
                    new Amount("10.83 USD"), false),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-16"),
                    new Amount("2000.99 USD"),
                    new Amount("50.21 EUR"), true),
                new Transaction(partitionKey, Guid.NewGuid(), DateTime.Parse("2022-05-18"),
                    new Amount("1.00 PLN"),
                    Amount.Zero, true)
            };
            var previousYearsCosts = 999m;

            var report = TaxReport.Generate(partitionKey, Guid.NewGuid(), transactions, exchangeRates, previousYearsCosts);
            var expectedPreviousYearsCosts = 999m;
            var expectedRevenue = 13947.9m;
            var expectedIncome = 10284.93m;
            var expectedCosts = 2663.97m;
            var expectedTax = 1954;
            var expectedCurrentYearCosts = 0;
            var expectedProfits1 = 0.0m;
            var expoectedCosts1 = 960.37m;
            var expectedProfits2 = 0.0m;
            var expectedCosts2 = 1311.96m;
            var expectedProfits3 = 13946.90m;
            var expectedCosts3 = 391.64m;
            var expectedProfits4 = 1;
            var expectedCosts4 = 0;

            report.PreviousYearsCosts.Should().Be(expectedPreviousYearsCosts);
            report.Revenue.Should().Be(expectedRevenue);
            report.Costs.Should().Be(expectedCosts);
            report.Income.Should().Be(expectedIncome);
            report.CurrentYearCosts.Should().Be(expectedCurrentYearCosts);
            report.Tax.Should().Be(expectedTax);
            report.Transactions.ToList()[0].CalculateProfits().Should().Be(expectedProfits1);
            report.Transactions.ToList()[0].CalculateCosts().Should().Be(expoectedCosts1);
            report.Transactions.ToList()[1].CalculateProfits().Should().Be(expectedProfits2);
            report.Transactions.ToList()[1].CalculateCosts().Should().Be(expectedCosts2);
            report.Transactions.ToList()[2].CalculateProfits().Should().Be(expectedProfits3);
            report.Transactions.ToList()[2].CalculateCosts().Should().Be(expectedCosts3);
            report.Transactions.ToList()[3].CalculateProfits().Should().Be(expectedProfits4);
            report.Transactions.ToList()[3].CalculateCosts().Should().Be(expectedCosts4);
        }
    }
}