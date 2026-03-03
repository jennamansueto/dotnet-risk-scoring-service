using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Contoso.RiskScoring.Api
{
    // TODO: Migration — in .NET 8 this becomes builder.Services.AddControllers() + app.MapControllers()
    // in Program.cs. Route configuration moves to attribute routing exclusively.
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.Converters.Add(new StringEnumConverter());

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Filters.Add(new Filters.GlobalExceptionFilterAttribute());
            config.MessageHandlers.Add(new Handlers.CorrelationIdHandler());
        }
    }
}
