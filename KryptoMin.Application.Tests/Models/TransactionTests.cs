using System;
using System.Collections.Generic;
using FluentAssertions;
using KryptoMin.Application.Models;
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
}