using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Application.Services;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;
using Contoso.RiskScoring.Infrastructure.Configuration;
using Contoso.RiskScoring.Infrastructure.Repositories;

namespace Contoso.RiskScoring.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRiskScoringServices(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerProfileRepository, InMemoryCustomerProfileRepository>();

            services.AddSingleton<RiskScoringEngine>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RiskScoringOptions>>().Value;

                var rules = new List<IRiskRule>
                {
                    new HighAmountRule(options.HighAmountThreshold, options.ExtremeAmountThreshold),
                    new HighRiskCountryRule(options.HighRiskCountries, options.ElevatedRiskCountries),
                    new UnusualMerchantCategoryRule(),
                    new CustomerRiskTierRule(),
                    new SpendDeviationRule(options.SpendDeviationMultiplier)
                };

                return new RiskScoringEngine(rules, options.ReviewThreshold, options.DeclineThreshold);
            });

            services.AddScoped<IRiskScoringService, RiskScoringService>();

            return services;
        }
    }
}
