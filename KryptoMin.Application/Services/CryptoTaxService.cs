using KryptoMin.Application.Contracts;
using KryptoMin.Application.Dtos;
using KryptoMin.Application.Models;

namespace KryptoMin.Application.Services
{
    public class CryptoTaxService : ICryptoTaxService
    {
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public CryptoTaxService(IExchangeRateProvider exchangeRateProvider)
        {
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<TaxReportDto> GenerateReport(TaxReportRequestDto request)
        {
            var transactions = request.Transactions.Select(x =>
                new Transaction(x.Date, x.Method, new Amount(x.Amount), x.Price, new Amount(x.Fees), x.FinalAmount, x.IsSell, x.TransactionId)
            ).ToList();

            var requestsForAmounts = transactions.Select(x => new ExchangeRateRequest(x.Amount.Currency, x.FormattedDayBefore));
            var requestsForFees = transactions.Select(x => new ExchangeRateRequest(x.Fees.Currency, x.FormattedDayBefore));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var balance = 0.0m;
            var transactionsResponse = new List<TransactionResponseDto>();
            foreach (var transaction in transactions)
            {
                if (transaction.IsSell)
                {
                    var exchangeRateForAmount = GetExchangeRate(exchangeRates, transaction.Amount.Currency, transaction.FormattedDayBefore);
                    var exchangeRateForFees = GetExchangeRate(exchangeRates, transaction.Fees.Currency, transaction.FormattedDayBefore);
                    var costs = transaction.Fees.Value * exchangeRateForFees.Value;
                    var profits = transaction.Amount.Value * exchangeRateForAmount.Value;
                    balance -= costs;
                    balance += profits;
                    transactionsResponse.Add(MapToResponseDto(transaction, exchangeRateForAmount, exchangeRateForFees, costs, profits));
                }
                else
                {
                    var exchangeRateForAmount = GetExchangeRate(exchangeRates, transaction.Amount.Currency, transaction.FormattedDayBefore);
                    var exchangeRateForFees = GetExchangeRate(exchangeRates, transaction.Fees.Currency, transaction.FormattedDayBefore);
                    var costs = transaction.Amount.Value * exchangeRateForAmount.Value + transaction.Fees.Value * exchangeRateForFees.Value;
                    balance -= costs;
                    transactionsResponse.Add(MapToResponseDto(transaction, exchangeRateForAmount, exchangeRateForFees, costs));
                }
            }
    
            balance = Math.Round(balance, 2);
            var balanceWithPreviousYearLoss = Math.Round(balance - request.PreviousYearLoss, 2);
            var tax = balanceWithPreviousYearLoss > 0 ? Math.Round(balanceWithPreviousYearLoss * 0.19m, 2) : 0;

            return new TaxReportDto
            {
                Tax = tax,
                Transactions= transactionsResponse,
                Balance = balance,
                BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss,
                PreviousYearLoss = request.PreviousYearLoss
            };
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.Date == date);
        }

        private TransactionResponseDto MapToResponseDto(Transaction transaction, ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, decimal costs, decimal profits = 0m)
        {
            return new TransactionResponseDto
            {
                Amount = transaction.Amount,
                Costs = costs,
                Profits = profits,
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