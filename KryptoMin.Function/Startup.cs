using System;
using KryptoMin.Application.Contracts;
using KryptoMin.Application.Services;
using KryptoMin.Application.Settings;
using KryptoMin.Domain.Entities;
using KryptoMin.Domain.Repositories;
using KryptoMin.Infra.Abstract;
using KryptoMin.Infra.HttpClients;
using KryptoMin.Infra.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KryptoMin.Function.Startup))]

namespace KryptoMin.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<INbpHttpClient, NbpHttpClient>();
            builder.Services.AddScoped<IExchangeRateProvider, NbpExchangeRateProvider>();
            builder.Services.AddScoped<ICryptoTaxService, CryptoTaxService>();
            builder.Services.AddScoped<IRepository<TaxReport>, AzureTableStorageRepository>();
            builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();

            var emailSettings = new EmailSettings(Environment.GetEnvironmentVariable("SendGridApiKey"));
            builder.Services.AddSingleton<EmailSettings>(emailSettings);

            var dbSettings = new DbSettings(Environment.GetEnvironmentVariable("StorageConnectionString"));
            builder.Services.AddSingleton<DbSettings>(dbSettings);
        }
    }
}