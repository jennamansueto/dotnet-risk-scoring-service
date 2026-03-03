using System;

namespace Contoso.RiskScoring.Domain.Entities
{
    public class TransactionContext
    {
        public Guid TransactionId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string MerchantCategory { get; set; }
        public string Country { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public CustomerProfile CustomerProfile { get; set; }
    }
}
