using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.RiskScoring.Api.Handlers
{
    // TODO: Migration — replace with ASP.NET Core middleware. DelegatingHandler is a
    // System.Net.Http concept; in Core you use IMiddleware or convention-based middleware.
    public class CorrelationIdHandler : DelegatingHandler
    {
        private const string Header = "X-Correlation-Id";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string correlationId;

            if (request.Headers.TryGetValues(Header, out var values) && values.Any())
            {
                correlationId = values.First();
            }
            else
            {
                correlationId = Guid.NewGuid().ToString("D");
                request.Headers.Add(Header, correlationId);
            }

            request.Properties["CorrelationId"] = correlationId;
            Trace.TraceInformation("[{0}] {1} {2}", correlationId, request.Method, request.RequestUri);

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.Headers.Contains(Header))
                response.Headers.Add(Header, correlationId);

            return response;
        }
    }
}
