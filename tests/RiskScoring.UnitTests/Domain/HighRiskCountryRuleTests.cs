using System;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    [TestClass]
    public class HighRiskCountryRuleTests
    {
        private readonly HighRiskCountryRule _sut = new HighRiskCountryRule(
            new[] { "KP", "IR", "SY" },
            new[] { "NG", "RU" });

        [TestMethod]
        public void Evaluate_SafeCountry_ReturnsZero()
        {
            var outcome = _sut.Evaluate(CreateContext("US"));
            Assert.AreEqual(0, outcome.ScoreContribution);
            Assert.IsNull(outcome.Reason);
        }

        [TestMethod]
        public void Evaluate_HighRiskCountry_Returns30()
        {
            var outcome = _sut.Evaluate(CreateContext("KP"));
            Assert.AreEqual(30, outcome.ScoreContribution);
            Assert.IsNotNull(outcome.Reason);
        }

        [TestMethod]
        public void Evaluate_ElevatedRiskCountry_Returns15()
        {
            var outcome = _sut.Evaluate(CreateContext("NG"));
            Assert.AreEqual(15, outcome.ScoreContribution);
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
