using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    public class HighAmountRule : IRiskRule
    {
        private readonly decimal _threshold;
        private readonly decimal _extremeThreshold;

        public HighAmountRule(decimal threshold, decimal extremeThreshold)
        {
            _threshold = threshold;
            _extremeThreshold = extremeThreshold;
        }

        public RuleOutcome Evaluate(TransactionContext context)
        {
            if (context.Amount >= _extremeThreshold)
                return new RuleOutcome(35,
                    string.Format("Transaction amount {0:C} exceeds extreme threshold ({1:C})", context.Amount, _extremeThreshold));

            if (context.Amount >= _threshold)
                return new RuleOutcome(20,
                    string.Format("Transaction amount {0:C} exceeds high-value threshold ({1:C})", context.Amount, _threshold));

            return new RuleOutcome(0, null);
        }
    }
}
