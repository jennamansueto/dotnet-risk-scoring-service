using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Contoso.RiskScoring.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string Header = "X-Correlation-Id";
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId;

            if (context.Request.Headers.TryGetValue(Header, out var values) && values.Count > 0)
            {
                correlationId = values[0];
            }
            else
            {
                correlationId = Guid.NewGuid().ToString("D");
                context.Request.Headers[Header] = correlationId;
            }

            context.Items["CorrelationId"] = correlationId;

            _logger.LogInformation("[{CorrelationId}] {Method} {Path}",
                correlationId, context.Request.Method, context.Request.Path);

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(Header))
                    context.Response.Headers[Header] = correlationId;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
