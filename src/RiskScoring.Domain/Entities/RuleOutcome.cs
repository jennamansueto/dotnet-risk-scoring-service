namespace Contoso.RiskScoring.Domain.Entities
{
    public class RuleOutcome
    {
        public int ScoreContribution { get; set; }
        public string Reason { get; set; }

        public RuleOutcome(int scoreContribution, string reason)
        {
            ScoreContribution = scoreContribution;
            Reason = reason;
        }
    }
}
