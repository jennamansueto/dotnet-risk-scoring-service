using System;
using System.Configuration;
using System.Linq;

namespace Contoso.RiskScoring.Infrastructure.Configuration
{
    // TODO: Migration — replace ConfigurationManager with IConfiguration / IOptions<T> in .NET 8.
    // This static helper is a common legacy pattern that makes unit testing difficult.
    public static class AppSettingsReader
    {
        public static string[] GetCsvSetting(string key, string defaultValue)
        {
            var raw = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(raw))
                raw = defaultValue;

            return raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => s.Trim())
                      .ToArray();
        }

        public static int GetIntSetting(string key, int defaultValue)
        {
            var raw = ConfigurationManager.AppSettings[key];
            int result;
            return int.TryParse(raw, out result) ? result : defaultValue;
        }

        public static decimal GetDecimalSetting(string key, decimal defaultValue)
        {
            var raw = ConfigurationManager.AppSettings[key];
            decimal result;
            return decimal.TryParse(raw, out result) ? result : defaultValue;
        }
    }
}
