using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    public class SpendDeviationRule : IRiskRule
    {
        private readonly decimal _deviationMultiplier;

        public SpendDeviationRule(decimal deviationMultiplier)
        {
            _deviationMultiplier = deviationMultiplier;
        }

        public RuleOutcome Evaluate(TransactionContext context)
        {
            if (context.CustomerProfile == null || context.CustomerProfile.AverageMonthlySpend <= 0)
                return new RuleOutcome(0, null);

            if (context.Amount > context.CustomerProfile.AverageMonthlySpend * _deviationMultiplier)
                return new RuleOutcome(15,
                    string.Format("Transaction amount is >{0}x customer average monthly spend ({1:C})",
                        _deviationMultiplier, context.CustomerProfile.AverageMonthlySpend));

            return new RuleOutcome(0, null);
        }
    }
}
