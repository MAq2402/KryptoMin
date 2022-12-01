using System;
using FluentAssertions;
using KryptoMin.Domain.ValueObjects;
using Xunit;

namespace KryptoMin.Domain.Tests.ValueObjects
{
    public class ExchangeRateTests
    {
        [Theory]
        [InlineData(1, "1", 2022, 09, 15, "PLN", "1,1,2022-09-15,PLN")]
        [InlineData(111.15, "1/2r",  2022, 09, 15, "USD", "111.15,1/2r,2022-09-15,USD")]
        public void ToString_ShouldWork(decimal value, string number, int year, int month, int day, string currency, string result)
        {
            var exchaneRate = new ExchangeRate(value, number, new DateTime(year, month, day), currency);
            exchaneRate.ToString().Should().Be(result);
        }

        [Theory]
        [InlineData(1, "1", 2022, 09, 15, "PLN", "1,1,2022-09-15,PLN")]
        [InlineData(111.15, "1/2r",  2022, 09, 15, "USD", "111.15,1/2r,2022-09-15,USD")]
        public void ExplicitFromString_ShouldWork(decimal value, string number, int year, int month, int day, string currency, string exchaneRateAsString)
        {
            var exchaneRate = new ExchangeRate(value, number, new DateTime(year, month, day), currency);
            ((ExchangeRate)exchaneRateAsString).Should().Be(exchaneRate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExplicitFromString_ShouldReturnNull(string exchaneRateAsString)
        {
            ((ExchangeRate)exchaneRateAsString).Should().Be(null);
        }
    }
}