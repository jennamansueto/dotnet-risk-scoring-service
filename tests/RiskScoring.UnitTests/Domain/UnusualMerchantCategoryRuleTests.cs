using System;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    [TestClass]
    public class UnusualMerchantCategoryRuleTests
    {
        private readonly UnusualMerchantCategoryRule _sut = new UnusualMerchantCategoryRule();

        [TestMethod]
        public void Evaluate_NormalMcc_ReturnsZero()
        {
            var outcome = _sut.Evaluate(CreateContext("5411"));
            Assert.AreEqual(0, outcome.ScoreContribution);
        }

        [TestMethod]
        public void Evaluate_GamblingMcc_Returns20()
        {
            var outcome = _sut.Evaluate(CreateContext("7995"));
            Assert.AreEqual(20, outcome.ScoreContribution);
            Assert.IsNotNull(outcome.Reason);
        }

        [TestMethod]
        public void Evaluate_CryptoMcc_Returns20()
        {
            var outcome = _sut.Evaluate(CreateContext("6051"));
            Assert.AreEqual(20, outcome.ScoreContribution);
        }

        private static TransactionContext CreateContext(string mcc)
        {
            return new TransactionContext
            {
                TransactionId = Guid.NewGuid(),
                CustomerId = "CUST-001",
                Amount = 100m,
                Currency = "USD",
                MerchantCategory = mcc,
                Country = "US",
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
