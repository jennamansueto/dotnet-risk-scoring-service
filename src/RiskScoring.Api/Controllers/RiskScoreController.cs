using System;
using System.Net;
using System.Web.Http;
using Contoso.RiskScoring.Application.DTOs;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Infrastructure.Configuration;

namespace Contoso.RiskScoring.Api.Controllers
{
    // TODO: Migration — in .NET 8 inherit from ControllerBase, use [ApiController] attribute,
    // and inject IRiskScoringService via constructor DI instead of using the static CompositionRoot.
    [RoutePrefix("api/risk-score")]
    public class RiskScoreController : ApiController
    {
        private readonly IRiskScoringService _service;

        public RiskScoreController()
        {
            // Poor man's DI — typical of legacy Web API 2 without a container.
            // TODO: Migration — replace with constructor injection from IServiceProvider.
            _service = CompositionRoot.CreateRiskScoringService();
        }

        // Overload for unit testing
        public RiskScoreController(IRiskScoringService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Score([FromBody] TransactionRiskRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (request.TransactionId == Guid.Empty)
                return BadRequest("transactionId is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerId))
                return BadRequest("customerId is required.");

            if (request.Amount <= 0)
                return BadRequest("amount must be greater than zero.");

            var response = _service.Evaluate(request);
            return Ok(response);
        }
    }
}
