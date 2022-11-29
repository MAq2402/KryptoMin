using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Services;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Enums;
using KryptoMin.Domain.Repositories;
using KryptoMin.Domain.ValueObjects;
using Moq;
using Xunit;
using System.Linq;

namespace KryptoMin.Application.Tests.Services
{
    public class ReportServiceTests
    {
        [Fact]
        public async Task Send_ShouldWork()
        {
            var exchangeRateProvider = new Mock<IExchangeRateProvider>();
            var emailSender = new Mock<IEmailSender>();
            var repository = new Mock<IRepository<TaxReport>>();
            var request = new SendReportRequestDto
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Email = "mail@test.com"
            };
            var taxReport = new TaxReport(Guid.NewGuid(), Guid.NewGuid(), new List<Transaction>(), 
                0, "", TaxReportStatus.Created);
            repository.Setup(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey))).ReturnsAsync(taxReport);
            var sut = new ReportService(emailSender.Object, repository.Object, exchangeRateProvider.Object);

            await sut.Send(request);

            repository.Verify(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey)), Times.Once);
            emailSender.Verify(x => x.Send(request.Email, taxReport), Times.Once);
            repository.Verify(x => x.Update(taxReport), Times.Once);
            taxReport.Status.Should().Be(TaxReportStatus.Received);
            taxReport.OwnerEmail.Should().Be("mail@test.com");
        }

        [Fact]
        public async Task Send_ShouldFail_ReportIsNull()
        {
            var exchangeRateProvider = new Mock<IExchangeRateProvider>();
            var emailSender = new Mock<IEmailSender>();
            var repository = new Mock<IRepository<TaxReport>>();
            var request = new SendReportRequestDto
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Email = "mail@test.com"
            };
            repository.Setup(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey))).ReturnsAsync(default(TaxReport));
            var sut = new ReportService(emailSender.Object, repository.Object, exchangeRateProvider.Object);

            await sut.Invoking(x => x.Send(request)).Should().ThrowExactlyAsync<ArgumentNullException>("Report with given ids has not been found");

            repository.Verify(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey)), Times.Once);
            emailSender.Verify(x => x.Send(request.Email, It.IsAny<TaxReport>()), Times.Never);
            repository.Verify(x => x.Update(It.IsAny<TaxReport>()), Times.Never);
        }

        [Fact]
        public async Task Send_ShouldFail_FailedToSend()
        {
            var exchangeRateProvider = new Mock<IExchangeRateProvider>();
            var emailSender = new Mock<IEmailSender>();
            var repository = new Mock<IRepository<TaxReport>>();
            var request = new SendReportRequestDto
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Email = "mail@test.com"
            };
            var taxReport = new TaxReport(Guid.NewGuid(), Guid.NewGuid(), new List<Transaction>(), 
                0, "", TaxReportStatus.Created);
            repository.Setup(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey))).ReturnsAsync(taxReport);
            emailSender.Setup(x => x.Send(request.Email, taxReport)).ThrowsAsync(new Exception());
            var sut = new ReportService(emailSender.Object, repository.Object, exchangeRateProvider.Object);

            await sut.Invoking(x => x.Send(request)).Should().ThrowExactlyAsync<Exception>("Report with given ids has not been found");

            repository.Verify(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey)), Times.Once);
            emailSender.Verify(x => x.Send(request.Email, taxReport), Times.Once);
            repository.Verify(x => x.Update(taxReport), Times.Once);

            taxReport.Status.Should().Be(TaxReportStatus.FailedToSend);
            taxReport.OwnerEmail.Should().Be("mail@test.com");
        }

        [Fact]
        public async Task GenerateReport_ShouldWork()
        {
            var exchangeRateProvider = new Mock<IExchangeRateProvider>();
            var reportRepository = new Mock<IRepository<TaxReport>>();
            var emailSender = new Mock<IEmailSender>();
            var exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate(1, "1", DateTime.Parse("2022-05-17"), "PLN"),
                new ExchangeRate(2.53m, "2", DateTime.Parse("2022-05-16"), "EUR"),
                new ExchangeRate(4.21m, "3", DateTime.Parse("2022-05-16"), "USD"),
                new ExchangeRate(6.97m, "4", DateTime.Parse("2022-05-13"), "USD"),
                new ExchangeRate(7.80m, "5", DateTime.Parse("2022-05-13"), "EUR"),
            };

            exchangeRateProvider.Setup(x => x.Get(It.IsAny<IEnumerable<ExchangeRateRequestDto>>())).ReturnsAsync(exchangeRates);
            var sut = new ReportService(emailSender.Object, reportRepository.Object, exchangeRateProvider.Object);

            var taxReportRequest = new GenerateRequestDto
            {
                PreviousYearLoss = 999m,
                Transactions = new List<TransactionDto>
                {
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-18"),
                        Amount = "941.54 PLN",
                        Fees = "18.83 PLN",
                        IsSell = false
                    },
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-17"),
                        Amount = "500.54 EUR",
                        Fees = "10.83 USD",
                        IsSell = false
                    },
                    new TransactionDto
                    {
                        Date = DateTime.Parse("2022-05-16"),
                        Amount = "2000.99 USD",
                        Fees = string.Empty,
                        IsSell = true
                    }
                }
            };

            var actual = await sut.Generate(taxReportRequest);
    
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "PLN" && x.Date == "2022-05-17"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "EUR" && x.Date == "2022-05-16"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-16"))), Times.Once);
            exchangeRateProvider.Verify(x => x.Get(It.Is<IEnumerable<ExchangeRateRequestDto>>(x => x.Any(x => x.Currency == "USD" && x.Date == "2022-05-13"))), Times.Once);
            reportRepository.Verify(x => x.Add(It.IsAny<TaxReport>()), Times.Once);
            actual.Should().NotBeNull();
        }
    }
}