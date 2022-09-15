using System;
using FluentAssertions;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Domain.Tests.ValueObjects;

public class AmountTests
{
    [Theory]
    [InlineData("825.25 PLN", 825.25, "PLN")]
    [InlineData("125.25 USD", 125.25, "USD")]
    [InlineData("850.00 EUR", 850, "EUR")]
    public void Creation_Should_Work(string input, decimal expectedValue, string expectedCurrency)
    {
        var amount = new Amount(input);

        amount.Currency.Should().Be(expectedCurrency);
        amount.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("850.00EUR")]
    [InlineData("1500.00")]
    public void Creation_Should_Fail(string input)
    {
        Action act = () => new Amount(input);

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData("825,15 PLN", 82515, "PLN")]
    [InlineData("125,30 USD", 12530, "USD")]
    [InlineData("850,00 EUR", 85000, "EUR")]
    public void Creation_Should_Fail_Wrong_Value(string input, decimal expectedValue, string expectedCurrency)
    {
        var amount = new Amount(input);

        amount.Currency.Should().Be(expectedCurrency);
        amount.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("825,15 PLN", "825,15 PLN", true)]
    [InlineData("825,15 USD", "825,15 PLN", false)]
    [InlineData("825,15 PLN", "100,15 PLN", false)]
    public void Compare_ShouldWork(string value1, string value2, bool expected)
    {
        var amount1 = new Amount(value1);
        var amount2 = new Amount(value2);

        (amount1 == amount2).Should().Be(expected);
    }

    [Theory]
    [InlineData("825.15 PLN")]
    [InlineData("100.15 USD")]
    [InlineData("0.15 EUR")]
    public void ToString_ShouldWork(string value)
    {
        var amount = new Amount(value);

        amount.ToString().Should().Be(value);
    }
}