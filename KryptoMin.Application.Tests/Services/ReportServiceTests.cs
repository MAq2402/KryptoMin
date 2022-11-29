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
using Moq;
using Xunit;

namespace KryptoMin.Application.Tests.Services
{
    public class ReportServiceTests
    {
        [Fact]
        public async Task Send_ShouldWork()
        {
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
            var sut = new ReportService(emailSender.Object, repository.Object);

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
            var emailSender = new Mock<IEmailSender>();
            var repository = new Mock<IRepository<TaxReport>>();
            var request = new SendReportRequestDto
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Email = "mail@test.com"
            };
            repository.Setup(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey))).ReturnsAsync(default(TaxReport));
            var sut = new ReportService(emailSender.Object, repository.Object);

            await sut.Invoking(x => x.Send(request)).Should().ThrowExactlyAsync<ArgumentNullException>("Report with given ids has not been found");

            repository.Verify(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey)), Times.Once);
            emailSender.Verify(x => x.Send(request.Email, It.IsAny<TaxReport>()), Times.Never);
            repository.Verify(x => x.Update(It.IsAny<TaxReport>()), Times.Never);
        }

        [Fact]
        public async Task Send_ShouldFail_FailedToSend()
        {
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
            var sut = new ReportService(emailSender.Object, repository.Object);

            await sut.Invoking(x => x.Send(request)).Should().ThrowExactlyAsync<Exception>("Report with given ids has not been found");

            repository.Verify(x => x.Get(new Guid(request.PartitionKey), new Guid(request.RowKey)), Times.Once);
            emailSender.Verify(x => x.Send(request.Email, taxReport), Times.Once);
            repository.Verify(x => x.Update(taxReport), Times.Once);

            taxReport.Status.Should().Be(TaxReportStatus.FailedToSend);
            taxReport.OwnerEmail.Should().Be("mail@test.com");
        }
    }
}