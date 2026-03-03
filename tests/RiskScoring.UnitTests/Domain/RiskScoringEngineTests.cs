using System;
using System.Collections.Generic;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Enums;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    [TestClass]
    public class RiskScoringEngineTests
    {
        [TestMethod]
        public void Evaluate_LowRiskTransaction_ReturnsApprove()
        {
            var engine = CreateEngine();
            var context = CreateContext(500m, "US", "5411");

            var result = engine.Evaluate(context);

            Assert.AreEqual(RiskDecision.Approve, result.Decision);
            Assert.IsTrue(result.Score < 40);
        }

        [TestMethod]
        public void Evaluate_HighAmountFromHighRiskCountry_ReturnsDecline()
        {
            var engine = CreateEngine();
            var context = CreateContext(60000m, "KP", "7995");

            var result = engine.Evaluate(context);

            Assert.AreEqual(RiskDecision.Decline, result.Decision);
            Assert.IsTrue(result.Score >= 70);
            Assert.IsTrue(result.Reasons.Count >= 3);
        }

        [TestMethod]
        public void Evaluate_MediumRisk_ReturnsReview()
        {
            var engine = CreateEngine();
            var context = CreateContext(15000m, "NG", "5411");

            var result = engine.Evaluate(context);

            Assert.AreEqual(RiskDecision.Review, result.Decision);
            Assert.IsTrue(result.Score >= 40 && result.Score < 70);
        }

        [TestMethod]
        public void Evaluate_ScoreClampedToMax100()
        {
            var engine = CreateEngine();
            var context = CreateContext(60000m, "KP", "7995");
            context.CustomerProfile = new CustomerProfile
            {
                CustomerId = "CUST-003",
                RiskTier = RiskTier.High,
                AverageMonthlySpend = 500m,
                AccountOpenedDate = DateTimeOffset.UtcNow.AddMonths(-3)
            };

            var result = engine.Evaluate(context);

            Assert.IsTrue(result.Score <= 100);
        }

        [TestMethod]
        public void Evaluate_PreservesTransactionId()
        {
            var engine = CreateEngine();
            var txId = Guid.NewGuid();
            var context = CreateContext(100m, "US", "5411");
            context.TransactionId = txId;

            var result = engine.Evaluate(context);

            Assert.AreEqual(txId, result.TransactionId);
        }

        private static RiskScoringEngine CreateEngine()
        {
            var rules = new List<IRiskRule>
            {
                new HighAmountRule(10000m, 50000m),
                new HighRiskCountryRule(
                    new[] { "KP", "IR", "SY", "CU", "VE", "MM", "BY" },
                    new[] { "NG", "PK", "UA", "RU", "AF" }),
                new UnusualMerchantCategoryRule(),
                new CustomerRiskTierRule(),
                new SpendDeviationRule(3.0m)
            };

            return new RiskScoringEngine(rules, reviewThreshold: 40, declineThreshold: 70);
        }

        private static TransactionContext CreateContext(decimal amount, string country, string mcc)
        {
            return new TransactionContext
            {
                TransactionId = Guid.NewGuid(),
                CustomerId = "CUST-001",
                Amount = amount,
                Currency = "USD",
                MerchantCategory = mcc,
                Country = country,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
