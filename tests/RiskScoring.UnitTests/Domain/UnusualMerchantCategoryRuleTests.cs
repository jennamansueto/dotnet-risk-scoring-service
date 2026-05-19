using System;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Rules;
using Xunit;

namespace Contoso.RiskScoring.UnitTests.Domain
{
    public class UnusualMerchantCategoryRuleTests
    {
        private readonly UnusualMerchantCategoryRule _sut = new UnusualMerchantCategoryRule();

        [Fact]
        public void Evaluate_NormalMcc_ReturnsZero()
        {
            var outcome = _sut.Evaluate(CreateContext("5411"));
            Assert.Equal(0, outcome.ScoreContribution);
        }

        [Fact]
        public void Evaluate_GamblingMcc_Returns20()
        {
            var outcome = _sut.Evaluate(CreateContext("7995"));
            Assert.Equal(20, outcome.ScoreContribution);
            Assert.NotNull(outcome.Reason);
        }

        [Fact]
        public void Evaluate_CryptoMcc_Returns20()
        {
            var outcome = _sut.Evaluate(CreateContext("6051"));
            Assert.Equal(20, outcome.ScoreContribution);
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
