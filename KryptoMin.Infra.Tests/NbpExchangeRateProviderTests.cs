using Xunit;
using KryptoMin.Infra.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using KryptoMin.Infra.Models.Nbp;
using KryptoMin.Infra.HttpClients;
using KryptoMin.Application.Dtos;

namespace KryptoMin.Infra.Tests;

public class NbpExchangeRateProviderTests
{
    [Fact]
    public async Task Get_Should_ReturnEmpty()
    {
        var mockedNbpHttpClient = new Mock<INbpHttpClient>();
        var sut = new NbpExchangeRateProvider(mockedNbpHttpClient.Object);

        var result = await sut.Get(new List<ExchangeRateRequestDto>());

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_Should_Work()
    {
        var mockedNbpHttpClient = new Mock<INbpHttpClient>();
        mockedNbpHttpClient.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ExchangeRatesResponse 
            { 
                Rates = new List<Rates>() { new Rates { Mid = 2.5m, No = "1" } 
            } 
        });
        var sut = new NbpExchangeRateProvider(mockedNbpHttpClient.Object);
        var requests = new List<ExchangeRateRequestDto>()
        {
            new ExchangeRateRequestDto("USD", "2022-01-10"),
            new ExchangeRateRequestDto("PLN", "2022-01-15"),
            new ExchangeRateRequestDto("PLN", "2022-01-10"),
            new ExchangeRateRequestDto("PLN", "2022-01-10"),
            new ExchangeRateRequestDto("USD", "2022-01-10"),
            new ExchangeRateRequestDto("EUR", "2022-01-02"),
        };

        var result = await sut.Get(requests);

        result.Should().HaveCount(4);
        mockedNbpHttpClient.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }
}