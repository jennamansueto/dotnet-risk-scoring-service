using System;
using Contoso.RiskScoring.Domain.Enums;

namespace Contoso.RiskScoring.Domain.Entities
{
    public class CustomerProfile
    {
        public string CustomerId { get; set; }
        public RiskTier RiskTier { get; set; }
        public decimal AverageMonthlySpend { get; set; }
        public DateTimeOffset AccountOpenedDate { get; set; }
    }
}
