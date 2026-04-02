using System.Threading.Tasks;
using Contoso.RiskScoring.Application.DTOs;

namespace Contoso.RiskScoring.Application.Interfaces
{
    public interface IRiskScoringService
    {
        Task<RiskScoreResponse> EvaluateAsync(TransactionRiskRequest request);
    }
}
