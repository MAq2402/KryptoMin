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
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 23),
            new Amount(amount), new Amount(fees), isSell);
        IEnumerable<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(exchangeRateForAmount, "1", new DateTime(2022, 7, 22), amountCurrency),
            new ExchangeRate(exchangeRateForFees, "2", new DateTime(2022, 7, 22), feesCurrency)
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
            new Amount("231.27 USD"),
            new Amount("231.27 USD"), isSell);

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
            new Amount("231.27 USD"),
            new Amount("231.27 USD"), isSell);

        Action act = () => transaction.CalculateProfits();

        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Theory]
    [InlineData("231.27 USD", "28.31 EUR", false, 2.5, 3.2, "USD", "EUR", 0)]
    [InlineData("282.31 EUR", "10.11 USD", true, 1.1, 7.2, "EUR", "USD", 310.54)]
    public void Transaction_CalculateProfits_ShouldWork(string amount, string fees, bool isSell,
        decimal exchangeRateForAmount, decimal exchangeRateForFees, string amountCurrency, string feesCurrency, decimal expectedProfits)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 23),
            new Amount(amount),
            new Amount(fees), isSell);
        IEnumerable<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(exchangeRateForAmount, "1", new DateTime(2022, 7, 22), amountCurrency),
            new ExchangeRate(exchangeRateForFees, "2", new DateTime(2022, 7, 22), feesCurrency)
        };
        transaction.AssignExchangeRates(exchangeRates);

        transaction.CalculateProfits().Should().Be(expectedProfits);
    }

    [Theory]
    [InlineData(2022, 7, 18, 2022, 07, 15)]
    [InlineData(2022, 7, 19, 2022, 07, 18)]
    [InlineData(2022, 7, 20, 2022, 07, 19)]
    [InlineData(2022, 7, 21, 2022, 07, 20)]
    [InlineData(2022, 7, 22, 2022, 07, 21)]
    [InlineData(2022, 7, 23, 2022, 07, 22)]
    [InlineData(2022, 7, 24, 2022, 07, 22)]
    public void Transaction_PreviousWorkingDay_ShouldWork(int year, int month, int day, int actualYear, int actualMonth, int actualDay)
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(year, month, day),
            new Amount("231.27 USD"),
            new Amount("28.31 EUR"), false);

        transaction.PreviousWorkingDay.Should().Be(new DateTime(actualYear, actualMonth, actualDay));
    }


    [Fact]
    public void AssignExchangeRates_ShouldWork_FeesNull()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 23),
            new Amount("231.27 USD"),
            Amount.Zero, false);

        List<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(3, "1", new DateTime(2022, 7, 21), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 21), "EUR"),
            new ExchangeRate(3, "1", new DateTime(2022, 7, 22), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 22), "EUR")
        };
        
        transaction.AssignExchangeRates(exchangeRates);

        transaction.ExchangeRateForAmount.Should().Be(exchangeRates[2]);
        transaction.ExchangeRateForFees.Should().BeNull();
    }

    [Fact]
    public void AssignExchangeRates_ShouldWork_FeesNotNull()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 23),
            new Amount("231.27 USD"),
            new Amount("28.31 EUR"), false);

        List<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(3, "1", new DateTime(2022, 7, 21), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 21), "EUR"),
            new ExchangeRate(3, "1", new DateTime(2022, 7, 22), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 22), "EUR")
        };
        
        transaction.AssignExchangeRates(exchangeRates);

        transaction.ExchangeRateForAmount.Should().Be(exchangeRates[2]);
        transaction.ExchangeRateForFees.Should().Be(exchangeRates[3]);
    }

    // Consider Domain Service for getting exchange rates that we are sure that the closest previous day is the correct day :D 
    // It might be overcomplicating things
    [Fact]
    public void AssignExchangeRates_ShouldFail_FeesNotNull()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 23),
            new Amount("231.27 USD"),
            new Amount("28.31 EUR"), false);

        List<ExchangeRate> exchangeRates = new List<ExchangeRate>()
        {
            new ExchangeRate(3, "1", new DateTime(2022, 7, 21), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 21), "EUR"),
            new ExchangeRate(3, "1", new DateTime(2022, 7, 22), "USD"),
            new ExchangeRate(2, "2", new DateTime(2022, 7, 23), "EUR")
        };
        
        Action action = () => transaction.AssignExchangeRates(exchangeRates);

        action.Should().Throw<Exception>();

        transaction.ExchangeRateForAmount.Should().Be(exchangeRates[2]);
    }
}