using System;
using System.Diagnostics;
using Contoso.RiskScoring.Application.DTOs;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;

namespace Contoso.RiskScoring.Application.Services
{
    // TODO: Migration — replace Trace.WriteLine with ILogger<T> in .NET 8.
    public class RiskScoringService : IRiskScoringService
    {
        private readonly RiskScoringEngine _engine;
        private readonly ICustomerProfileRepository _customerRepo;

        public RiskScoringService(RiskScoringEngine engine, ICustomerProfileRepository customerRepo)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
        }

        public RiskScoreResponse Evaluate(TransactionRiskRequest request)
        {
            Trace.TraceInformation(
                "Evaluating risk for transaction {0}, customer {1}, amount {2} {3}",
                request.TransactionId, request.CustomerId, request.Amount, request.Currency);

            var profile = _customerRepo.GetByCustomerId(request.CustomerId);

            var context = new TransactionContext
            {
                TransactionId = request.TransactionId,
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Currency = request.Currency,
                MerchantCategory = request.MerchantCategory,
                Country = request.Country,
                Timestamp = request.Timestamp,
                CustomerProfile = profile
            };

            var result = _engine.Evaluate(context);

            Trace.TraceInformation("Transaction {0} scored {1} -> {2}",
                result.TransactionId, result.Score, result.Decision);

            return new RiskScoreResponse
            {
                TransactionId = result.TransactionId,
                Score = result.Score,
                Decision = result.Decision.ToString().ToUpperInvariant(),
                Reasons = result.Reasons
            };
        }
    }
}
