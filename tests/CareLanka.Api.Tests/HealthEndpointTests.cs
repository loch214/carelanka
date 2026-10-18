using CareLanka.Api.DTOs.Common;
using CareLanka.Api.Services.Common;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CareLanka.Api.Tests;

public sealed class HealthEndpointTests
{
    [Fact]
    public async Task Health_endpoint_maps_a_healthy_service_result_to_200()
    {
        using var environment = TestEnvironment.Use();
        await using var application = new TestApplication();
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/health");
        var body = await response.Content.ReadFromJsonAsync<HealthStatus>();

        Assert.Equal(200, (int)response.StatusCode);
        Assert.Equal("healthy", body?.Status);
        Assert.Equal("up", body?.Database);
    }

    [Fact]
    public async Task Liveness_endpoint_answers_without_checking_the_database()
    {
        using var environment = TestEnvironment.Use();
        await using var application = new TestApplication(new UnreachableHealthService());
        using var client = application.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(200, (int)response.StatusCode);
    }

    private sealed class TestApplication(IHealthService? health = null) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHealthService>();
                services.AddSingleton(health ?? new StubHealthService());
            });
        }
    }

    private sealed class UnreachableHealthService : IHealthService
    {
        public Task<HealthStatus> GetHealthAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("The liveness endpoint must not check the database.");
    }

    private sealed class StubHealthService : IHealthService
    {
        public Task<HealthStatus> GetHealthAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new HealthStatus
            {
                Status = "healthy",
                Database = "up",
                Version = "test",
                CheckedAt = DateTimeOffset.UtcNow
            });
    }
}
