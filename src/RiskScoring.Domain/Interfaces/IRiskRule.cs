using Contoso.RiskScoring.Domain.Entities;

namespace Contoso.RiskScoring.Domain.Interfaces
{
    public interface IRiskRule
    {
        RuleOutcome Evaluate(TransactionContext context);
    }
}
