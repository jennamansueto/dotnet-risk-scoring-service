using System.Web.Http;
using Owin;

namespace Contoso.RiskScoring.Api
{
    // TODO: Migration — in .NET 8 this entire class disappears. The OWIN Startup is replaced
    // by WebApplication.CreateBuilder(args) in a top-level Program.cs. Middleware is registered
    // via app.UseMiddleware<T>() instead of message handlers and filter attributes.
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
