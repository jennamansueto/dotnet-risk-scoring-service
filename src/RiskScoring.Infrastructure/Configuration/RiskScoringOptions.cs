namespace Contoso.RiskScoring.Infrastructure.Configuration
{
    public class RiskScoringOptions
    {
        public int ReviewThreshold { get; set; } = 40;
        public int DeclineThreshold { get; set; } = 70;
        public decimal HighAmountThreshold { get; set; } = 10000m;
        public decimal ExtremeAmountThreshold { get; set; } = 50000m;
        public decimal SpendDeviationMultiplier { get; set; } = 3.0m;
        public string[] HighRiskCountries { get; set; } = new[] { "KP", "IR", "SY", "CU", "VE", "MM", "BY" };
        public string[] ElevatedRiskCountries { get; set; } = new[] { "NG", "PK", "UA", "RU", "AF" };
    }
}
