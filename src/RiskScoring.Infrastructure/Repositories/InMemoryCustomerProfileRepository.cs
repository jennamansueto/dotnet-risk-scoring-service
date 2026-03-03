using System;
using System.Collections.Generic;
using Contoso.RiskScoring.Domain.Entities;
using Contoso.RiskScoring.Domain.Enums;
using Contoso.RiskScoring.Domain.Interfaces;

namespace Contoso.RiskScoring.Infrastructure.Repositories
{
    // TODO: Migration — replace with EF Core or Dapper repository backed by a real database.
    // This in-memory stub exists because the original service read from a SQL Server stored proc
    // that was decommissioned. The fake data keeps the API functional for testing.
    public class InMemoryCustomerProfileRepository : ICustomerProfileRepository
    {
        private static readonly Dictionary<string, CustomerProfile> Profiles =
            new Dictionary<string, CustomerProfile>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "CUST-001", new CustomerProfile
                    {
                        CustomerId = "CUST-001",
                        RiskTier = RiskTier.Low,
                        AverageMonthlySpend = 2500m,
                        AccountOpenedDate = new DateTimeOffset(2019, 3, 15, 0, 0, 0, TimeSpan.Zero)
                    }
                },
                {
                    "CUST-002", new CustomerProfile
                    {
                        CustomerId = "CUST-002",
                        RiskTier = RiskTier.Medium,
                        AverageMonthlySpend = 8000m,
                        AccountOpenedDate = new DateTimeOffset(2021, 7, 1, 0, 0, 0, TimeSpan.Zero)
                    }
                },
                {
                    "CUST-003", new CustomerProfile
                    {
                        CustomerId = "CUST-003",
                        RiskTier = RiskTier.High,
                        AverageMonthlySpend = 500m,
                        AccountOpenedDate = new DateTimeOffset(2023, 11, 20, 0, 0, 0, TimeSpan.Zero)
                    }
                }
            };

        public CustomerProfile GetByCustomerId(string customerId)
        {
            CustomerProfile profile;
            Profiles.TryGetValue(customerId, out profile);
            return profile;
        }
    }
}
