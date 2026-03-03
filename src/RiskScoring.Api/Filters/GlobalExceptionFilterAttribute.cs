using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Diagnostics;

namespace Contoso.RiskScoring.Api.Filters
{
    // TODO: Migration — replace with ASP.NET Core middleware (ExceptionHandlingMiddleware)
    // that writes ProblemDetails JSON. The ExceptionFilterAttribute pattern does not exist in Core.
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Trace.TraceError("Unhandled exception in {0} {1}: {2}",
                context.Request.Method,
                context.Request.RequestUri,
                context.Exception);

            var response = new
            {
                error = "An unexpected error occurred.",
                detail = "An internal server error has occurred. Please contact support if the problem persists."
            };

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                response);
        }
    }
}
