using System.Threading.Tasks;
using Contoso.RiskScoring.Domain.Entities;

namespace Contoso.RiskScoring.Domain.Interfaces
{
    public interface ICustomerProfileRepository
    {
        Task<CustomerProfile> GetByCustomerIdAsync(string customerId);
    }
}
