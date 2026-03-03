using System;
using System.Collections.Generic;
using System.Linq;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Domain.Rules
{
    // TODO: Migration — these country lists are currently loaded from Web.config appSettings
    // via ConfigurationManager. In .NET 8 move to appsettings.json with IOptions<T>.
    public class HighRiskCountryRule : IRiskRule
    {
        private readonly HashSet<string> _highRiskCountries;
        private readonly HashSet<string> _elevatedRiskCountries;

        public HighRiskCountryRule(IEnumerable<string> highRiskCountries, IEnumerable<string> elevatedRiskCountries)
        {
            _highRiskCountries = new HashSet<string>(highRiskCountries, StringComparer.OrdinalIgnoreCase);
            _elevatedRiskCountries = new HashSet<string>(elevatedRiskCountries, StringComparer.OrdinalIgnoreCase);
        }

        public RuleOutcome Evaluate(TransactionContext context)
        {
            if (_highRiskCountries.Contains(context.Country))
                return new RuleOutcome(30,
                    string.Format("Transaction originates from high-risk country ({0})", context.Country));

            if (_elevatedRiskCountries.Contains(context.Country))
                return new RuleOutcome(15,
                    string.Format("Transaction originates from elevated-risk country ({0})", context.Country));

            return new RuleOutcome(0, null);
        }
    }
}
