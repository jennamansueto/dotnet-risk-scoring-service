# Contoso.RiskScoring.Legacy

A transaction risk scoring microservice built on **.NET Framework 4.7.2** using **ASP.NET Web API 2**. Evaluates real-time transaction risk using a rules-based scoring engine and returns a decision (APPROVE / REVIEW / DECLINE).

> **Note:** This is the pre-migration legacy version. See [`docs/migration-plan.md`](docs/migration-plan.md) for the plan to migrate to .NET 8.

## Architecture

```
src/
  RiskScoring.Api              ASP.NET Web API 2 (OWIN self-host, controllers, handlers, filters)
  RiskScoring.Application      Orchestration layer (DTOs, IRiskScoringService)
  RiskScoring.Domain           Core domain (entities, rules engine, no external deps)
  RiskScoring.Infrastructure   Implementations (in-memory repos, ConfigurationManager, CompositionRoot)
tests/
  RiskScoring.UnitTests        MSTest unit tests for domain rules and engine
docs/
  architecture.md              System architecture and data flow
  migration-plan.md            Step-by-step .NET 8 migration plan
build/
  azure-pipelines.yml          CI pipeline (Windows agent, VSBuild)
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (any recent version — builds `net472` via reference assemblies)
- Alternatively: Visual Studio 2022 with .NET Framework 4.7.2 targeting pack

## Build & Test

```bash
dotnet restore Contoso.RiskScoring.Legacy.sln
dotnet build Contoso.RiskScoring.Legacy.sln --configuration Release
dotnet test Contoso.RiskScoring.Legacy.sln --configuration Release
```

## Configuration

Settings are in `src/RiskScoring.Api/App.config`:

```xml
<appSettings>
  <add key="ReviewThreshold" value="40" />
  <add key="DeclineThreshold" value="70" />
  <add key="HighAmountThreshold" value="10000" />
  <add key="ExtremeAmountThreshold" value="50000" />
  <add key="SpendDeviationMultiplier" value="3.0" />
  <add key="HighRiskCountries" value="KP,IR,SY,CU,VE,MM,BY" />
  <add key="ElevatedRiskCountries" value="NG,PK,UA,RU,AF" />
</appSettings>
```

## API Endpoints

### Health Check

```
GET /api/health
```

### Score a Transaction

```
POST /api/risk-score
Content-Type: application/json
```

## Sample Requests

### Low-risk transaction (expect APPROVE)

```bash
curl -s -X POST http://localhost:9000/api/risk-score \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: test-001" \
  -d '{
    "transactionId": "a1b2c3d4-0000-0000-0000-000000000001",
    "customerId": "CUST-001",
    "amount": 250.00,
    "currency": "USD",
    "merchantCategory": "5411",
    "country": "US",
    "timestamp": "2024-12-01T10:30:00Z"
  }'
```

### High-risk transaction (expect DECLINE)

```bash
curl -s -X POST http://localhost:9000/api/risk-score \
  -H "Content-Type: application/json" \
  -d '{
    "transactionId": "a1b2c3d4-0000-0000-0000-000000000002",
    "customerId": "CUST-003",
    "amount": 75000.00,
    "currency": "USD",
    "merchantCategory": "7995",
    "country": "KP",
    "timestamp": "2024-12-01T10:30:00Z"
  }'
```

### Medium-risk transaction (expect REVIEW)

```bash
curl -s -X POST http://localhost:9000/api/risk-score \
  -H "Content-Type: application/json" \
  -d '{
    "transactionId": "a1b2c3d4-0000-0000-0000-000000000003",
    "customerId": "CUST-002",
    "amount": 15000.00,
    "currency": "EUR",
    "merchantCategory": "5411",
    "country": "NG",
    "timestamp": "2024-12-01T10:30:00Z"
  }'
```

## Risk Rules

| Rule | Trigger | Score |
|------|---------|-------|
| High Amount | >= $10K / >= $50K | +20 / +35 |
| High-Risk Country | Sanctioned (KP, IR...) / Elevated (NG, RU...) | +30 / +15 |
| Unusual MCC | Gambling, crypto, wire transfers | +20 |
| Customer Risk Tier | High / Medium / Unknown | +25 / +10 / +10 |
| Spend Deviation | > 3x average monthly spend | +15 |

### Decision Thresholds

| Score Range | Decision |
|-------------|----------|
| 0–39 | APPROVE |
| 40–69 | REVIEW |
| 70–100 | DECLINE |

## Legacy Characteristics

This codebase reflects common .NET Framework patterns:
- **Manual DI** via static `CompositionRoot` (no IoC container)
- **ConfigurationManager** for all settings (`App.config` / `Web.config`)
- **Newtonsoft.Json** for serialization
- **DelegatingHandler** for cross-cutting concerns (correlation ID)
- **ExceptionFilterAttribute** for global error handling
- **Synchronous** controller actions and repository calls
- **System.Diagnostics.Trace** for logging

See `TODO` comments throughout the code for specific migration pain points.

## License

Internal use only — Contoso Ltd.
