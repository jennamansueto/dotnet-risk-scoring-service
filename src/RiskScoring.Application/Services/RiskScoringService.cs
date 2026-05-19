using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Contoso.RiskScoring.Application.DTOs;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;

namespace Contoso.RiskScoring.Application.Services
{
    public class RiskScoringService : IRiskScoringService
    {
        private readonly RiskScoringEngine _engine;
        private readonly ICustomerProfileRepository _customerRepo;
        private readonly ILogger<RiskScoringService> _logger;

        public RiskScoringService(
            RiskScoringEngine engine,
            ICustomerProfileRepository customerRepo,
            ILogger<RiskScoringService> logger)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RiskScoreResponse> EvaluateAsync(TransactionRiskRequest request)
        {
            _logger.LogInformation(
                "Evaluating risk for transaction {TransactionId}, customer {CustomerId}, amount {Amount} {Currency}",
                request.TransactionId, request.CustomerId, request.Amount, request.Currency);

            var profile = await _customerRepo.GetByCustomerIdAsync(request.CustomerId).ConfigureAwait(false);

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

            _logger.LogInformation("Transaction {TransactionId} scored {Score} -> {Decision}",
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
