using System.Collections.Generic;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Application.Services;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;
using Contoso.RiskScoring.Infrastructure.Repositories;

namespace Contoso.RiskScoring.Infrastructure.Configuration
{
    // TODO: Migration — replace this manual wiring with IServiceCollection / AddScoped / AddSingleton
    // in .NET 8 Program.cs. This "poor man's DI" was typical in Web API 2 projects that didn't
    // want to pull in Unity or Autofac for a small service.
    public static class CompositionRoot
    {
        public static IRiskScoringService CreateRiskScoringService()
        {
            var highRiskCountries = AppSettingsReader.GetCsvSetting("HighRiskCountries", "KP,IR,SY,CU,VE,MM,BY");
            var elevatedRiskCountries = AppSettingsReader.GetCsvSetting("ElevatedRiskCountries", "NG,PK,UA,RU,AF");
            var highAmountThreshold = AppSettingsReader.GetDecimalSetting("HighAmountThreshold", 10000m);
            var extremeAmountThreshold = AppSettingsReader.GetDecimalSetting("ExtremeAmountThreshold", 50000m);
            var spendDeviationMultiplier = AppSettingsReader.GetDecimalSetting("SpendDeviationMultiplier", 3.0m);
            var reviewThreshold = AppSettingsReader.GetIntSetting("ReviewThreshold", 40);
            var declineThreshold = AppSettingsReader.GetIntSetting("DeclineThreshold", 70);

            var rules = new List<IRiskRule>
            {
                new HighAmountRule(highAmountThreshold, extremeAmountThreshold),
                new HighRiskCountryRule(highRiskCountries, elevatedRiskCountries),
                new UnusualMerchantCategoryRule(),
                new CustomerRiskTierRule(),
                new SpendDeviationRule(spendDeviationMultiplier)
            };

            var engine = new RiskScoringEngine(rules, reviewThreshold, declineThreshold);
            var customerRepo = new InMemoryCustomerProfileRepository();

            return new RiskScoringService(engine, customerRepo);
        }
    }
}
