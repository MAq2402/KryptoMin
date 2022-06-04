namespace KryptoMin.Infra.Models.Nbp
{
    public class ExchangeRatesResponse
    {
        public IEnumerable<Rates> Rates { get; set; }
    }

    public class Rates
    {
        public double Mid { get; set; }
        public string No { get; set; }
    }
}