using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Models;

namespace KryptoMin.Application.Services
{
    public class CryptoTaxService : ICryptoTaxService
    {
        private const decimal TaxRate = 0.19m;
        private const int DecimalPlaces = 2;
        private readonly IExchangeRateProvider _exchangeRateProvider;
        private readonly IReportRepository _reportRepository;

        public CryptoTaxService(IExchangeRateProvider exchangeRateProvider, IReportRepository reportRepository)
        {
            _exchangeRateProvider = exchangeRateProvider;
            _reportRepository = reportRepository;
        }

        public async Task<TaxReportDto> GenerateReport(TaxReportRequestDto request)
        {
            var transactions = request.Transactions.Select(x =>
                new Transaction(x.Date, x.Method, new Amount(x.Amount), x.Price, new Amount(x.Fees), x.FinalAmount, x.IsSell, x.TransactionId)
            ).ToList();

            var requestsForAmounts = transactions.Select(x => new ExchangeRateRequestDto(x.Amount.Currency, x.FormattedPreviousWorkingDay));
            var requestsForFees = transactions.Select(x => new ExchangeRateRequestDto(x.Fees.Currency, x.FormattedPreviousWorkingDay));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var balance = 0.0m;
            var transactionsResponse = new List<TransactionResponseDto>();
            foreach (var transaction in transactions)
            {
                var exchangeRateForAmount = GetExchangeRate(exchangeRates, transaction.Amount.Currency, transaction.FormattedPreviousWorkingDay);
                var exchangeRateForFees = GetExchangeRate(exchangeRates, transaction.Fees.Currency, transaction.FormattedPreviousWorkingDay);

                transaction.SetExchangeRates(exchangeRateForAmount, exchangeRateForFees);
                balance -= transaction.CalculateCosts();
                balance += transaction.CalculateProfits();

                transactionsResponse.Add(MapToResponseDto(transaction, exchangeRateForAmount, exchangeRateForFees));
            }
    
            balance = Math.Round(balance, DecimalPlaces);
            var balanceWithPreviousYearLoss = Math.Round(balance - request.PreviousYearLoss, DecimalPlaces);
            var tax = balanceWithPreviousYearLoss > 0 ? Math.Round(balanceWithPreviousYearLoss * TaxRate, DecimalPlaces) : 0;
  
            var report = new TaxReportDto
            {
                Tax = tax,
                Transactions= transactionsResponse,
                Balance = balance,
                BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss,
                PreviousYearLoss = request.PreviousYearLoss
            };

            await _reportRepository.Save(report);
            return report;
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.Date == date);
        }

        private TransactionResponseDto MapToResponseDto(Transaction transaction, ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees)
        {
            return new TransactionResponseDto
            {
                Amount = transaction.Amount,
                Costs = transaction.Costs,
                Profits = transaction.Profits,
                Date = transaction.Date,
                ExchangeRateAmount = exchangeRateForAmount,
                ExchangeRateFees = exchangeRateForFees,
                Fees = transaction.Fees,
                FinalAmount = transaction.FinalAmount,
                Method = transaction.Method,
                Price = transaction.Price,
                IsSell = transaction.IsSell,
                TransactionId = transaction.TransactionId
            };
        }
    }
}