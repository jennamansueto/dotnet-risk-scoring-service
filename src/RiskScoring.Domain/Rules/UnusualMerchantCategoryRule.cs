using System;
using System.Collections.Generic;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    public class UnusualMerchantCategoryRule : IRiskRule
    {
        private static readonly HashSet<string> HighRiskMccs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "7995", // Gambling
            "6051", // Crypto / quasi-cash
            "5967", // Direct marketing — inbound teleservices
            "5816", // Digital goods
            "4829"  // Wire transfers / money orders
        };

        public RuleOutcome Evaluate(TransactionContext context)
        {
            if (HighRiskMccs.Contains(context.MerchantCategory))
                return new RuleOutcome(20,
                    string.Format("Merchant category code {0} is flagged as high-risk", context.MerchantCategory));

            return new RuleOutcome(0, null);
        }
    }
}
