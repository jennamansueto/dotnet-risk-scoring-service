using Contoso.RiskScoring.Domain.Entities;

namespace Contoso.RiskScoring.Domain.Interfaces
{
    public interface ICustomerProfileRepository
    {
        // TODO: Migration — make this async (Task<CustomerProfile>) when moving to .NET 8.
        // Kept synchronous here because the legacy codebase uses synchronous controllers.
        CustomerProfile GetByCustomerId(string customerId);
    }
}
