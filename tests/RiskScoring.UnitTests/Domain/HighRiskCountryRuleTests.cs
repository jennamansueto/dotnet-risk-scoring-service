using System;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Rules;
using Xunit;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    public class HighRiskCountryRuleTests
    {
        private readonly HighRiskCountryRule _sut = new HighRiskCountryRule(
            new[] { "KP", "IR", "SY" },
            new[] { "NG", "RU" });

        [Fact]
        public void Evaluate_SafeCountry_ReturnsZero()
        {
            var outcome = _sut.Evaluate(CreateContext("US"));
            Assert.Equal(0, outcome.ScoreContribution);
            Assert.Null(outcome.Reason);
        }

        [Fact]
        public void Evaluate_HighRiskCountry_Returns30()
        {
            var outcome = _sut.Evaluate(CreateContext("KP"));
            Assert.Equal(30, outcome.ScoreContribution);
            Assert.NotNull(outcome.Reason);
        }

        [Fact]
        public void Evaluate_ElevatedRiskCountry_Returns15()
        {
            var outcome = _sut.Evaluate(CreateContext("NG"));
            Assert.Equal(15, outcome.ScoreContribution);
        }

        private static TransactionContext CreateContext(string country)
        {
            return new TransactionContext
            {
                TransactionId = Guid.NewGuid(),
                CustomerId = "CUST-001",
                Amount = 100m,
                Currency = "USD",
                MerchantCategory = "5411",
                Country = country,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
