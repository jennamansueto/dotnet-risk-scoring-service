# Architecture — Contoso.RiskScoring.Legacy

## Overview

The Risk Scoring Service evaluates real-time transaction risk and returns a decision (APPROVE / REVIEW / DECLINE) based on a configurable rules engine. It runs on **.NET Framework 4.7.2** using **ASP.NET Web API 2** hosted via OWIN self-host (or IIS in production).

## Solution Structure

```
src/
  RiskScoring.Api              ASP.NET Web API 2 host
    App_Start/WebApiConfig.cs  Route registration, JSON formatting, filters, handlers
    Startup.cs                 OWIN startup class
    Controllers/               RiskScoreController, HealthController
    Filters/                   GlobalExceptionFilterAttribute
    Handlers/                  CorrelationIdHandler (DelegatingHandler)
    App.config                 appSettings for thresholds and country lists

  RiskScoring.Application      Orchestration layer
    DTOs/                      TransactionRiskRequest, RiskScoreResponse
    Interfaces/                IRiskScoringService
    Services/                  RiskScoringService (coordinates repo + engine)

  RiskScoring.Domain           Core domain (zero external dependencies)
    Entities/                  TransactionContext, CustomerProfile, RiskResult, RuleOutcome
    Enums/                     RiskDecision, RiskTier
    Interfaces/                IRiskRule, ICustomerProfileRepository
    Rules/                     HighAmountRule, HighRiskCountryRule, UnusualMerchantCategoryRule,
                               CustomerRiskTierRule, SpendDeviationRule, RiskScoringEngine

  RiskScoring.Infrastructure   Implementations + wiring
    Repositories/              InMemoryCustomerProfileRepository
    Configuration/             AppSettingsReader (static ConfigurationManager wrapper),
                               CompositionRoot (manual DI)

tests/
  RiskScoring.UnitTests        MSTest tests for domain rules and engine
```

## Key Design Decisions

### Manual Dependency Injection
The service uses a static `CompositionRoot` class instead of an IoC container. This was a deliberate choice to avoid adding Unity/Autofac for a small service, but it makes testing harder and couples the API controller to concrete infrastructure.

### Configuration via ConfigurationManager
All tunable values (thresholds, country lists) live in `App.config` / `Web.config` `<appSettings>`. The `AppSettingsReader` static helper wraps `ConfigurationManager.AppSettings` lookups. This is the standard .NET Framework pattern but is not testable without config file manipulation.

### Synchronous API
The entire pipeline is synchronous. The repository, service, and controller all use synchronous methods. This was acceptable when the repo was backed by a fast in-memory store, but would need to become async for any real I/O.

### Logging via System.Diagnostics.Trace
Logging uses `Trace.TraceInformation` / `Trace.TraceError`. There is no structured logging. Log output depends on configured `TraceListener` instances.

## Data Flow

```
HTTP Request
  -> CorrelationIdHandler (sets X-Correlation-Id)
  -> RiskScoreController.Score()
  -> CompositionRoot.CreateRiskScoringService()  [poor man's DI]
  -> RiskScoringService.Evaluate()
    -> ICustomerProfileRepository.GetByCustomerId()
    -> RiskScoringEngine.Evaluate()
      -> HighAmountRule
      -> HighRiskCountryRule
      -> UnusualMerchantCategoryRule
      -> CustomerRiskTierRule
      -> SpendDeviationRule
    -> Aggregate score, determine decision
  -> RiskScoreResponse (JSON via Newtonsoft.Json)
```

## Risk Rules

| Rule | Trigger | Score |
|------|---------|-------|
| High Amount | >= $10K / >= $50K | +20 / +35 |
| High-Risk Country | Sanctioned / Elevated | +30 / +15 |
| Unusual MCC | Gambling, crypto, wires | +20 |
| Customer Tier | High / Medium / Unknown | +25 / +10 / +10 |
| Spend Deviation | > 3x avg monthly spend | +15 |

### Decision Thresholds (configurable via appSettings)

| Score | Decision |
|-------|----------|
| 0–39 | APPROVE |
| 40–69 | REVIEW |
| 70–100 | DECLINE |
