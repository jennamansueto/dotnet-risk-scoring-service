using Contoso.RiskScoring.Application.DTOs;

namespace Contoso.RiskScoring.Application.Interfaces
{
    // TODO: Migration — make this async (Task<RiskScoreResponse>) for .NET 8.
    public interface IRiskScoringService
    {
        RiskScoreResponse Evaluate(TransactionRiskRequest request);
    }
}
