using Xunit;
using KryptoMin.Infra.Services;
using Moq;
using System.Collections.Generic;
using KryptoMin.Application.Models;
using System.Threading.Tasks;
using FluentAssertions;
using System;
using KryptoMin.Infra.Models.Nbp;

namespace KryptoMin.Infra.Tests;

public class NbpExchangeRateProviderTests
{
    [Fact]
    public async Task Get_Should_ReturnEmpty()
    {
        var mockedNbpHttpClient = new Mock<INbpHttpClient>();
        var sut = new NbpExchangeRateProvider(mockedNbpHttpClient.Object);

        var result = await sut.Get(new List<ExchangeRateRequest>());

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_Should_Work()
    {
        var mockedNbpHttpClient = new Mock<INbpHttpClient>();
        mockedNbpHttpClient.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ExchangeRatesResponse 
            { 
                Rates = new List<Rates>() { new Rates { Mid = 2.5, No = "1" } 
            } 
        });
        var sut = new NbpExchangeRateProvider(mockedNbpHttpClient.Object);
        var requests = new List<ExchangeRateRequest>()
        {
            new ExchangeRateRequest("USD", "2022-10-01"),
            new ExchangeRateRequest("PLN", "2022-15-01"),
            new ExchangeRateRequest("PLN", "2022-10-01"),
            new ExchangeRateRequest("PLN", "2022-10-01"),
            new ExchangeRateRequest("USD", "2022-10-01"),
            new ExchangeRateRequest("EUR", "2022-02-01"),
        };

        var result = await sut.Get(requests);

        result.Should().HaveCount(4);
        mockedNbpHttpClient.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }
}