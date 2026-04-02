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
    public class RiskScoreApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RiskScoreApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Score_ValidRequest_ReturnsOk()
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

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            Assert.True(root.TryGetProperty("score", out var scoreElement));
            Assert.True(root.TryGetProperty("decision", out var decisionElement));
            Assert.Equal("APPROVE", decisionElement.GetString());
        }

        [Fact]
        public async Task Score_HighRiskTransaction_ReturnsDecline()
        {
            var request = new
            {
                transactionId = Guid.NewGuid(),
                customerId = "CUST-001",
                amount = 60000m,
                currency = "USD",
                merchantCategory = "7995",
                country = "KP",
                timestamp = DateTimeOffset.UtcNow
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            Assert.Equal("DECLINE", root.GetProperty("decision").GetString());
        }

        [Fact]
        public async Task Score_NullBody_ReturnsBadRequest()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/risk-score", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Score_ReturnsCorrelationIdHeader()
        {
            var request = new
            {
                transactionId = Guid.NewGuid(),
                customerId = "CUST-001",
                amount = 100m,
                currency = "USD",
                merchantCategory = "5411",
                country = "US",
                timestamp = DateTimeOffset.UtcNow
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/risk-score", content);

            Assert.True(response.Headers.Contains("X-Correlation-Id"));
        }
    }
}
