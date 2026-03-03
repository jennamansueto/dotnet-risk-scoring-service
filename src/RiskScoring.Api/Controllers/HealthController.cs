using System.Web.Http;

namespace Contoso.RiskScoring.Api.Controllers
{
    // TODO: Migration — replace with app.MapHealthChecks("/health") in .NET 8.
    // The built-in health check middleware supports liveness/readiness probes natively.
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
