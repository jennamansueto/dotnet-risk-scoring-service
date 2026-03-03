using System;
using System.Collections.Generic;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Enums;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    // TODO: Migration — consider making this an injectable service behind an interface
    // and using IServiceCollection in .NET 8 instead of manual construction.
    public class RiskScoringEngine
    {
        private readonly IReadOnlyList<IRiskRule> _rules;
        private readonly int _reviewThreshold;
        private readonly int _declineThreshold;

        public RiskScoringEngine(IEnumerable<IRiskRule> rules, int reviewThreshold, int declineThreshold)
        {
            _rules = new List<IRiskRule>(rules);
            _reviewThreshold = reviewThreshold;
            _declineThreshold = declineThreshold;
        }

        public RiskResult Evaluate(TransactionContext context)
        {
            var reasons = new List<string>();
            int totalScore = 0;

            foreach (var rule in _rules)
            {
                var outcome = rule.Evaluate(context);
                totalScore += outcome.ScoreContribution;
                if (outcome.Reason != null)
                    reasons.Add(outcome.Reason);
            }

            int clampedScore = Math.Min(Math.Max(totalScore, 0), 100);

            RiskDecision decision;
            if (clampedScore >= _declineThreshold)
                decision = RiskDecision.Decline;
            else if (clampedScore >= _reviewThreshold)
                decision = RiskDecision.Review;
            else
                decision = RiskDecision.Approve;

            return new RiskResult
            {
                TransactionId = context.TransactionId,
                Score = clampedScore,
                Decision = decision,
                Reasons = reasons
            };
        }
    }
}
