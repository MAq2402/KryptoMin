using KryptoMin.Application.Contracts;
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
            var purchases = request.Transactions.Select(x =>
                new Purchase(x.Date, x.Method, new Amount(x.Amount), x.Price, new Amount(x.Fees), x.FinalAmount, x.IsSell, x.TransactionId)
            ).ToList();

            var requestsForAmounts = purchases.Select(x => new ExchangeRateRequest(x.Amount.Currency, x.FormattedDayBefore));
            var requestsForFees = purchases.Select(x => new ExchangeRateRequest(x.Fees.Currency, x.FormattedDayBefore));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            var balance = 0.0m;
            var purchasesResponse = new List<PurchaseResponseDto>();
            foreach (var purchase in purchases)
            {
                if (purchase.IsSell)
                {
                    var exchangeRateForAmount = GetExchangeRate(exchangeRates, purchase.Amount.Currency, purchase.FormattedDayBefore);
                    var exchangeRateForFees = GetExchangeRate(exchangeRates, purchase.Fees.Currency, purchase.FormattedDayBefore);
                    var costs = purchase.Fees.Value * exchangeRateForFees.Value;
                    var profits = purchase.Amount.Value * exchangeRateForAmount.Value;
                    balance -= costs;
                    balance += profits;
                    purchasesResponse.Add(MapToResponseDto(purchase, exchangeRateForAmount, exchangeRateForFees, costs, profits));
                }
                else
                {
                    var exchangeRateForAmount = GetExchangeRate(exchangeRates, purchase.Amount.Currency, purchase.FormattedDayBefore);
                    var exchangeRateForFees = GetExchangeRate(exchangeRates, purchase.Fees.Currency, purchase.FormattedDayBefore);
                    var costs = purchase.Amount.Value * exchangeRateForAmount.Value + purchase.Fees.Value * exchangeRateForFees.Value;
                    balance -= costs;
                    purchasesResponse.Add(MapToResponseDto(purchase, exchangeRateForAmount, exchangeRateForFees, costs));
                }
            }
    
            balance = Math.Round(balance, 2);
            var balanceWithPreviousYearLoss = Math.Round(balance - request.PreviousYearLoss, 2);
            var tax = balanceWithPreviousYearLoss > 0 ? Math.Round(balanceWithPreviousYearLoss * 0.19m, 2) : 0;

            return new TaxReportDto
            {
                Tax = tax,
                Purchases = purchasesResponse,
                Balance = balance,
                BalanceWithPreviousYearLoss = balanceWithPreviousYearLoss,
                PreviousYearLoss = request.PreviousYearLoss
            };
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.Date == date);
        }

        private PurchaseResponseDto MapToResponseDto(Purchase purchase, ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, decimal costs, decimal profits = 0m)
        {
            return new PurchaseResponseDto
            {
                Amount = purchase.Amount,
                Costs = costs,
                Profits = profits,
                Date = purchase.Date,
                ExchangeRateAmount = exchangeRateForAmount,
                ExchangeRateFees = exchangeRateForFees,
                Fees = purchase.Fees,
                FinalAmount = purchase.FinalAmount,
                Method = purchase.Method,
                Price = purchase.Price,
                IsSell = purchase.IsSell,
                TransactionId = purchase.TransactionId
            };
        }
    }
}