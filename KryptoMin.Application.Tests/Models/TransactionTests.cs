using System;
using FluentAssertions;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Application.Tests.Models;

public class TransactionTests
{

    [Theory]
    [InlineData("231.27 USD", "28.31 EUR", false, 2.5, 3.2, 668.767)]
    [InlineData("282.31 EUR", "10.11 USD", true, 1.1, 7.2, 72.792)]
    public void Transaction_CalculateCosts_ShouldWork(string amount, string fees, bool isSell, 
        decimal exchangeRateForAmount, decimal exchangeRateForFees, decimal expectedCost)
    {
        var transaction = new Transaction(DateTime.Now, 
            "Credit Card", new Amount(amount), "4.61356493 USDT/PLN", 
            new Amount(fees), "200 USDT", isSell, "01223522377463013376051811");
        transaction.SetExchangeRates(new ExchangeRate(exchangeRateForAmount, "1", DateTime.Now.ToString(), "XXX"), 
            new ExchangeRate(exchangeRateForFees, "2", DateTime.Now.ToString(), "XXX"));

        transaction.CalculateCosts().Should().Be(expectedCost);
    }

    [Theory]
    [InlineData("231.27 USD", "28.31 EUR", false, 2.5, 3.2, 0)]
    [InlineData("282.31 EUR", "10.11 USD", true, 1.1, 7.2, 310.541)]
    public void Transaction_CalculateProfits_ShouldWork(string amount, string fees, bool isSell, 
        decimal exchangeRateForAmount, decimal exchangeRateForFees, decimal expectedProfits)
    {
        var transaction = new Transaction(DateTime.Now, 
            "Credit Card", new Amount(amount), "4.61356493 USDT/PLN", 
            new Amount(fees), "200 USDT", isSell, "01223522377463013376051811");
        transaction.SetExchangeRates(new ExchangeRate(exchangeRateForAmount, "1", DateTime.Now.ToString(), "XXX"), 
            new ExchangeRate(exchangeRateForFees, "2", DateTime.Now.ToString(), "XXX"));

        transaction.CalculateProfits().Should().Be(expectedProfits);
    }

    [Theory]
    [InlineData(2022, 7, 18, "2022-07-15")]
    [InlineData(2022, 7, 19, "2022-07-18")]
    [InlineData(2022, 7, 20, "2022-07-19")]
    [InlineData(2022, 7, 21, "2022-07-20")]
    [InlineData(2022, 7, 22, "2022-07-21")]
    [InlineData(2022, 7, 23, "2022-07-22")]
    [InlineData(2022, 7, 24, "2022-07-22")]
    public void Transaction_FormattedPreviousWorkingDay_ShouldWork(int year, int month, int day, string result)
    {
        var transaction = new Transaction(new DateTime(year, month, day),
            "Credit Card", new Amount("231.27 USD"), "4.61356493 USDT/PLN",
            new Amount("28.31 EUR"), "200 USDT", false, "01223522377463013376051811");

        transaction.FormattedPreviousWorkingDay.Should().Be(result);
    }
}