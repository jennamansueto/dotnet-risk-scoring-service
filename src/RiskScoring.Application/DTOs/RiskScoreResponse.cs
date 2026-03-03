using System;
using System.Collections.Generic;

namespace Contoso.RiskScoring.Application.DTOs
{
    public class RiskScoreResponse
    {
        public Guid TransactionId { get; set; }
        public int Score { get; set; }
        public string Decision { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();
    }
}
