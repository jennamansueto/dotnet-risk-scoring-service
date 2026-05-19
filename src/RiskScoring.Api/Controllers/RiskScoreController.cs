using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contoso.RiskScoring.Application.DTOs;
using Contoso.RiskScoring.Application.Interfaces;

namespace Contoso.RiskScoring.Api.Controllers
{
    [ApiController]
    [Route("api/risk-score")]
    public class RiskScoreController : ControllerBase
    {
        private readonly IRiskScoringService _service;

        public RiskScoreController(IRiskScoringService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("")]
        public async Task<ActionResult<RiskScoreResponse>> Score([FromBody] TransactionRiskRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.TransactionId == Guid.Empty)
                return BadRequest("transactionId is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerId))
                return BadRequest("customerId is required.");

            if (request.Amount <= 0)
                return BadRequest("amount must be greater than zero.");

            var response = await _service.EvaluateAsync(request).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
