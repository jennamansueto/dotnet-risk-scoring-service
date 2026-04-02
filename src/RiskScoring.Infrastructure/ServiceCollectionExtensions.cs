using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Contoso.RiskScoring.Application.Interfaces;
using Contoso.RiskScoring.Application.Services;
using Contoso.RiskScoring.Domain.Interfaces;
using Contoso.RiskScoring.Domain.Rules;
using Contoso.RiskScoring.Infrastructure.Configuration;
using Contoso.RiskScoring.Infrastructure.Repositories;

namespace Contoso.RiskScoring.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRiskScoringServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RiskScoringOptions>(configuration.GetSection("RiskScoring"));

            services.AddSingleton<ICustomerProfileRepository, InMemoryCustomerProfileRepository>();

            services.AddSingleton<IEnumerable<IRiskRule>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RiskScoringOptions>>().Value;
                return new List<IRiskRule>
                {
                    new HighAmountRule(options.HighAmountThreshold, options.ExtremeAmountThreshold),
                    new HighRiskCountryRule(options.HighRiskCountries, options.ElevatedRiskCountries),
                    new UnusualMerchantCategoryRule(),
                    new CustomerRiskTierRule(),
                    new SpendDeviationRule(options.SpendDeviationMultiplier)
                };
            });

            services.AddSingleton<RiskScoringEngine>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RiskScoringOptions>>().Value;
                var rules = sp.GetRequiredService<IEnumerable<IRiskRule>>();
                return new RiskScoringEngine(rules, options.ReviewThreshold, options.DeclineThreshold);
            });

            services.AddScoped<IRiskScoringService, RiskScoringService>();

            return services;
        }
    }
}
