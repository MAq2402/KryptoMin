using System;
using System.Collections.Generic;
using FluentAssertions;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Domain.Tests.Entities;

public class TransactionTests
{

    [Theory]
    [InlineData("231.27 USD", "28.31 EUR", false, 2.5, 3.2, "USD", "EUR", 668.77)]
    [InlineData("282.31 EUR", "10.11 USD", true, 1.1, 7.2, "EUR", "USD", 72.79)]
    public void Transaction_CalculateCosts_ShouldWork(string amount, string fees, bool isSell, 
        decimal exchangeRateForAmount, decimal exchangeRateForFees, string amountCurrency, string feesCurrency, decimal expectedCost)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 
            "Credit Card", new Amount(amount), "4.61356493 USDT/PLN", 
            new Amount(fees), "200 USDT", isSell, "01223522377463013376051811");
        IEnumerable<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(exchangeRateForAmount, "1", DateTime.Now.AddDays(-1), amountCurrency),
            new ExchangeRate(exchangeRateForFees, "2", DateTime.Now.AddDays(-1), feesCurrency)
        };
        transaction.AssignExchangeRates(exchangeRates);

        transaction.CalculateCosts().Should().Be(expectedCost);
    }

     public static IEnumerable<object[]> Transaction_CalculateCosts_ShouldThrowInvalidOperationException_Data
            => new object[][] {
                new object[] { new ExchangeRate(3.2m, "2", DateTime.Now, "XXX"), null, true },
                new object[] { null, new ExchangeRate(3.2m, "2", DateTime.Now, "XXX"), false }
    };

    [Theory]
    [MemberData(nameof(Transaction_CalculateCosts_ShouldThrowInvalidOperationException_Data))]
    public void Transaction_CalculateCosts_ShouldThrowInvalidOperationException(ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, bool isSell)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 
            "Credit Card", new Amount("231.27 USD"), "4.61356493 USDT/PLN", 
            new Amount("231.27 USD"), "200 USDT", isSell, "01223522377463013376051811");

        Action act = () => transaction.CalculateCosts();

        act.Should().ThrowExactly<InvalidOperationException>();
    }

    public static IEnumerable<object[]> Transaction_CalculateProfits_ShouldThrowInvalidOperationException_Data
            => new object[][] {
                new object[] { null, new ExchangeRate(3.2m, "2", DateTime.Now, "XXX"), true }
    };

    [Theory]
    [MemberData(nameof(Transaction_CalculateProfits_ShouldThrowInvalidOperationException_Data))]
    public void Transaction_CalculateProfits_ShouldThrowInvalidOperationException(ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, bool isSell)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 
            "Credit Card", new Amount("231.27 USD"), "4.61356493 USDT/PLN", 
            new Amount("231.27 USD"), "200 USDT", isSell, "01223522377463013376051811");

        Action act = () => transaction.CalculateProfits();

        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [InlineData("231.27 USD", "28.31 EUR", false, 2.5, 3.2, "USD", "EUR", 0)]
    [InlineData("282.31 EUR", "10.11 USD", true, 1.1, 7.2, "EUR", "USD", 310.54)]
    public void Transaction_CalculateProfits_ShouldWork(string amount, string fees, bool isSell, 
        decimal exchangeRateForAmount, decimal exchangeRateForFees, string amountCurrency, string feesCurrency,  decimal expectedProfits)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 
            "Credit Card", new Amount(amount), "4.61356493 USDT/PLN", 
            new Amount(fees), "200 USDT", isSell, "01223522377463013376051811");
        IEnumerable<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(exchangeRateForAmount, "1", DateTime.Now.AddDays(-1), amountCurrency),
            new ExchangeRate(exchangeRateForFees, "2", DateTime.Now.AddDays(-1), feesCurrency)
        };
        transaction.AssignExchangeRates(exchangeRates);

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
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(year, month, day),
            "Credit Card", new Amount("231.27 USD"), "4.61356493 USDT/PLN",
            new Amount("28.31 EUR"), "200 USDT", false, "01223522377463013376051811");

        transaction.FormattedPreviousWorkingDay.Should().Be(result);
    }
}