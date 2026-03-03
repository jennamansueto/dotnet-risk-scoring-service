using System;
using System.Collections.Generic;
using Contoso.RiskScoring.Domain.Enums;

namespace Contoso.RiskScoring.Domain.Entities
{
    public class RiskResult
    {
        public Guid TransactionId { get; set; }
        public int Score { get; set; }
        public RiskDecision Decision { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();
    }
}
