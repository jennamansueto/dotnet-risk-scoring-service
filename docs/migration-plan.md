# Migration Plan: .NET Framework 4.7.2 to .NET 8

## Executive Summary

Migrate the Risk Scoring Service from .NET Framework 4.7.2 (ASP.NET Web API 2) to .NET 8 (ASP.NET Core). This enables Linux/container deployment, modern DI, structured logging, and long-term support.

---

## Phase 1 — Preparation (Before Writing Any .NET 8 Code)

### 1.1 Audit NuGet Dependencies
- **Microsoft.AspNet.WebApi** → replaced by `Microsoft.AspNetCore.App` (framework reference)
- **Newtonsoft.Json** → evaluate switching to `System.Text.Json` (built-in); keep Newtonsoft only if custom converters are complex
- **Microsoft.Owin** → eliminated; Kestrel replaces OWIN self-host
- **System.Configuration.ConfigurationManager** → replaced by `IConfiguration` / `IOptions<T>`

### 1.2 Identify System.Web Dependencies
The current codebase uses:
- `System.Web.Http.ApiController` → `Microsoft.AspNetCore.Mvc.ControllerBase`
- `System.Web.Http.Filters.ExceptionFilterAttribute` → ASP.NET Core middleware
- `System.Net.Http.DelegatingHandler` → ASP.NET Core middleware
- `IHttpActionResult` → `IActionResult` / `ActionResult<T>`
- `HttpConfiguration` → `WebApplicationBuilder` / `IServiceCollection`

### 1.3 Catalog Configuration
All values in `App.config` `<appSettings>`:
| Key | Value | Migration Target |
|-----|-------|------------------|
| ReviewThreshold | 40 | `appsettings.json` → `RiskScoring:ReviewThreshold` |
| DeclineThreshold | 70 | `appsettings.json` → `RiskScoring:DeclineThreshold` |
| HighAmountThreshold | 10000 | `appsettings.json` |
| ExtremeAmountThreshold | 50000 | `appsettings.json` |
| SpendDeviationMultiplier | 3.0 | `appsettings.json` |
| HighRiskCountries | KP,IR,SY,... | `appsettings.json` (array) |
| ElevatedRiskCountries | NG,PK,UA,... | `appsettings.json` (array) |

Config transforms (Web.Debug.config, Web.Release.config) are replaced by:
- `appsettings.Development.json` / `appsettings.Production.json`
- Environment variables
- Azure App Configuration / Key Vault (if applicable)

---

## Phase 2 — Create .NET 8 Solution

### 2.1 Project Structure
Maintain clean architecture layers but use SDK-style csproj targeting `net8.0`:
```
src/RiskScoring.Api            → ASP.NET Core Web API (Program.cs, minimal hosting)
src/RiskScoring.Application    → Class library (unchanged interface, add async)
src/RiskScoring.Domain         → Class library (no changes expected)
src/RiskScoring.Infrastructure → Class library (DI via IServiceCollection)
tests/RiskScoring.UnitTests    → xUnit (replaces MSTest)
tests/RiskScoring.IntegrationTests → WebApplicationFactory<T> tests
```

### 2.2 Hosting Model
**Before (.NET Framework):**
```csharp
// Global.asax or OWIN Startup
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        var config = new HttpConfiguration();
        WebApiConfig.Register(config);
        app.UseWebApi(config);
    }
}
```

**After (.NET 8):**
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddRiskScoringServices(); // extension method

var app = builder.Build();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();
```

### 2.3 Dependency Injection
**Before:** `CompositionRoot.CreateRiskScoringService()` — static factory, called per-request.

**After:**
```csharp
services.AddSingleton<ICustomerProfileRepository, InMemoryCustomerProfileRepository>();
services.AddSingleton<IRiskRule, HighAmountRule>();
// ... register all rules
services.AddSingleton<RiskScoringEngine>();
services.AddScoped<IRiskScoringService, RiskScoringService>();
```

### 2.4 Configuration
**Before:** `ConfigurationManager.AppSettings["HighRiskCountries"]`

**After:**
```csharp
builder.Services.Configure<RiskScoringOptions>(
    builder.Configuration.GetSection("RiskScoring"));
```

### 2.5 Middleware
| Legacy (Web API 2) | .NET 8 Equivalent |
|--------------------|--------------------|
| `DelegatingHandler` (CorrelationIdHandler) | `app.UseMiddleware<CorrelationIdMiddleware>()` |
| `ExceptionFilterAttribute` | `app.UseMiddleware<ExceptionHandlingMiddleware>()` |
| `WebApiConfig.Register` filters | `builder.Services.AddControllers(options => ...)` |

### 2.6 Routing
**Before:** `[RoutePrefix("api/risk-score")]` + `config.MapHttpAttributeRoutes()`

**After:** `[Route("risk-score")]` with `[ApiController]` (no `/api` prefix — or keep it via convention). `MapControllers()` replaces explicit route registration.

### 2.7 Logging
**Before:** `Trace.TraceInformation(...)` with `System.Diagnostics.Trace`.

**After:** `ILogger<T>` injected via DI. Built-in JSON console logger. Structured log parameters.

### 2.8 Serialization
**Before:** Newtonsoft.Json configured in `WebApiConfig` with `CamelCasePropertyNamesContractResolver`.

**After:** `System.Text.Json` (default in ASP.NET Core). Add `JsonSerializerOptions` with `PropertyNamingPolicy = CamelCase` if needed.

---

## Phase 3 — Testing

### 3.1 Unit Tests
- Port MSTest tests to xUnit (or keep MSTest — both work on .NET 8)
- Domain rule tests should require minimal changes (no framework dependency)
- Application service tests need minor updates for async signatures

### 3.2 Integration Tests
- Use `WebApplicationFactory<Program>` to boot the API in-memory
- Test `POST /risk-score` end-to-end
- Test `/health` endpoint
- Verify correlation ID header flow

---

## Phase 4 — Cutover

1. Deploy .NET 8 service alongside legacy behind a load balancer / feature flag.
2. Shadow traffic: send duplicate requests to both, compare responses.
3. Gradually shift traffic from legacy to .NET 8.
4. Decommission legacy IIS deployment.
5. Remove config transforms and legacy CI pipeline.

---

## Risk Register

| Risk | Mitigation |
|------|-----------|
| Newtonsoft.Json serialization differences | Compare JSON output between old/new in shadow mode |
| ConfigurationManager behavior differences | Validate all settings load correctly in integration tests |
| Synchronous → async conversion bugs | Unit test all async paths; use ConfigureAwait(false) in libraries |
| Missing .NET Framework-only NuGet packages | Audit during Phase 1; find replacements early |
| IIS-specific behavior (e.g., request size limits) | Configure Kestrel limits to match IIS settings |
