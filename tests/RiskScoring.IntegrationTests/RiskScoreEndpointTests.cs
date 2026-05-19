using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Contoso.RiskScoring.IntegrationTests
{
    public class RiskScoreEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RiskScoreEndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostRiskScore_ValidRequest_ReturnsOk()
        {
            var request = new
            {
                transactionId = Guid.NewGuid(),
                customerId = "CUST-001",
                amount = 500m,
                currency = "USD",
                merchantCategory = "5411",
                country = "US",
                timestamp = DateTimeOffset.UtcNow
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal("APPROVE", doc.RootElement.GetProperty("decision").GetString());
        }

        [Fact]
        public async Task PostRiskScore_HighRisk_ReturnsDecline()
        {
            var request = new
            {
                transactionId = Guid.NewGuid(),
                customerId = "CUST-003",
                amount = 60000m,
                currency = "USD",
                merchantCategory = "7995",
                country = "KP",
                timestamp = DateTimeOffset.UtcNow
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal("DECLINE", doc.RootElement.GetProperty("decision").GetString());
        }

        [Fact]
        public async Task PostRiskScore_MissingBody_ReturnsBadRequest()
        {
            var content = new StringContent(
                "{}",
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CorrelationIdHeader_IsReturned()
        {
            var correlationId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Get, "/health");
            request.Headers.Add("X-Correlation-Id", correlationId);

            var response = await _client.SendAsync(request);
            Assert.True(response.Headers.Contains("X-Correlation-Id"));
            Assert.Contains(correlationId, response.Headers.GetValues("X-Correlation-Id"));
        }

        [Fact]
        public async Task CorrelationIdHeader_GeneratedWhenMissing()
        {
            var response = await _client.GetAsync("/health");
            Assert.True(response.Headers.Contains("X-Correlation-Id"));
        }
    }
}
