using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Enums;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    public class CustomerRiskTierRule : IRiskRule
    {
        public RuleOutcome Evaluate(TransactionContext context)
        {
            if (context.CustomerProfile == null)
                return new RuleOutcome(10, "Customer profile not available; defaulting to elevated risk");

            switch (context.CustomerProfile.RiskTier)
            {
                case RiskTier.High:
                    return new RuleOutcome(25, "Customer is classified as high-risk tier");
                case RiskTier.Medium:
                    return new RuleOutcome(10, "Customer is classified as medium-risk tier");
                default:
                    return new RuleOutcome(0, null);
            }
        }
    }
}
