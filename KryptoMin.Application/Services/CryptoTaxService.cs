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

        // WHAT ABOUT YEAR PREVOIS LOSS?
        // process only completed?
        //calculate +
        //decimals and double thing
        public async Task<TaxReportDto> GenerateReport(IEnumerable<PurchaseDto> purchasesDto)
        {
            var purchases = purchasesDto.Select(x =>
                new Purchase(x.Date, x.Method, new Amount(x.Amount), x.Price, new Amount(x.Fees), x.FinalAmount, x.TransactionId)
            ).ToList();

            var requestsForAmounts = purchases.Select(x => new ExchangeRateRequest(x.Amount.Currency, x.FormattedDayBefore));
            var requestsForFees = purchases.Select(x => new ExchangeRateRequest(x.Fees.Currency, x.FormattedDayBefore));
            var exchangeRates = await _exchangeRateProvider.Get(requestsForAmounts.Concat(requestsForFees));

            //NEXT STEPS TESTS
            //ONE LOOP - NO LOOP IN EXCHANGE Rate provider.I think i stay with two loops
            //calculate - From selling fees ,FROM BUYING AMOUNT From buying FEES
            // some logic include in purchase
            var balance = 0.0m;
            var purchasesResponse = new List<PurchaseResponseDto>();
            foreach(var purchase in purchases)
            {
                var exchangeRateForAmount = GetExchangeRate(exchangeRates, purchase.Amount.Currency, purchase.FormattedDayBefore);
                var exchangeRateForFees = GetExchangeRate(exchangeRates, purchase.Fees.Currency, purchase.FormattedDayBefore);
                var sumOfPurchaseCost = Decimal.ToDouble(purchase.Amount.Value) * exchangeRateForAmount.Value + Decimal.ToDouble(purchase.Fees.Value) * exchangeRateForFees.Value;
                balance -= (decimal)sumOfPurchaseCost;
                purchasesResponse.Add(MapToResponseDto(purchase, exchangeRateForAmount, exchangeRateForFees, sumOfPurchaseCost));
            }
            var tax = balance > 0 ? balance * 0.17m : 0;

            return new TaxReportDto
            {
                Tax = tax,
                Purchases = purchasesResponse,
                Balance = balance
            };
        }

        private ExchangeRate GetExchangeRate(IEnumerable<ExchangeRate> exchangeRates, string currency, string date)
        {
            return exchangeRates.First(x => x.Currency == currency && x.Date == date);
        }

        private PurchaseResponseDto MapToResponseDto(Purchase purchase, ExchangeRate exchangeRateForAmount, ExchangeRate exchangeRateForFees, double sumOfPurchaseCost)
        {
            return new PurchaseResponseDto
            {
                Amount = purchase.Amount,
                AmountInPln = sumOfPurchaseCost,
                Date = purchase.Date,
                ExchangeRateAmount = exchangeRateForAmount,
                ExchangeRateFees = exchangeRateForFees,
                Fees = purchase.Fees,
                FinalAmount = purchase.FinalAmount,
                Method = purchase.Method,
                Price = purchase.Price,
                TransactionId = purchase.TransactionId
            };
        }

    }
}