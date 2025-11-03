using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Mottu.Api.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");
  }
}

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
  private readonly WebApplicationFactory<Program> _factory;

  public ApiIntegrationTests(CustomWebApplicationFactory factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task Get_HealthCheckEndpoint_DeveRetornarOk()
  {
    var client = _factory.CreateClient();

    var response = await client.GetAsync("/health");

    response.EnsureSuccessStatusCode();
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task Get_MotosEndpoint_SemToken_DeveRetornarUnauthorized()
  {
    var client = _factory.CreateClient();

    var response = await client.GetAsync("/api/v1/Motos");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }
}
