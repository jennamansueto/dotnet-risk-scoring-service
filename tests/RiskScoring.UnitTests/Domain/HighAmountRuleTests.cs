using System;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    [TestClass]
    public class HighAmountRuleTests
    {
        private readonly HighAmountRule _sut = new HighAmountRule(10000m, 50000m);

        [TestMethod]
        public void Evaluate_BelowThreshold_ReturnsZeroScore()
        {
            var context = CreateContext(500m);
            var outcome = _sut.Evaluate(context);
            Assert.AreEqual(0, outcome.ScoreContribution);
            Assert.IsNull(outcome.Reason);
        }

        [TestMethod]
        public void Evaluate_AtThreshold_Returns20()
        {
            var context = CreateContext(10000m);
            var outcome = _sut.Evaluate(context);
            Assert.AreEqual(20, outcome.ScoreContribution);
            Assert.IsNotNull(outcome.Reason);
        }

        [TestMethod]
        public void Evaluate_AboveExtremeThreshold_Returns35()
        {
            var context = CreateContext(75000m);
            var outcome = _sut.Evaluate(context);
            Assert.AreEqual(35, outcome.ScoreContribution);
        }

        [TestMethod]
        public void Evaluate_BetweenThresholds_Returns20()
        {
            var context = CreateContext(25000m);
            var outcome = _sut.Evaluate(context);
            Assert.AreEqual(20, outcome.ScoreContribution);
        }

        private static TransactionContext CreateContext(decimal amount)
        {
            return new TransactionContext
            {
                TransactionId = Guid.NewGuid(),
                CustomerId = "CUST-001",
                Amount = amount,
                Currency = "USD",
                MerchantCategory = "5411",
                Country = "US",
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
