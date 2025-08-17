using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AgriFieldHub.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AgriFieldHub.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => { });
    }

    [Fact]
    public async Task Register_Then_Login_Returns_Jwt()
    {
        var client = _factory.CreateClient();

        var register = new RegisterRequest { Email = $"test_{Guid.NewGuid():N}@example.com", Password = "Passw0rd!" };
        var regResp = await client.PostAsJsonAsync("/api/auth/register", register);
        regResp.EnsureSuccessStatusCode();
        var regData = await regResp.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrEmpty(regData?.Token));

        var login = new LoginRequest { Email = register.Email, Password = register.Password };
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", login);
        loginResp.EnsureSuccessStatusCode();
        var loginData = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrEmpty(loginData?.Token));

        // Use token on authorized endpoint (fields list -> should be empty)
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.Token);
        var fieldsResp = await client.GetAsync("/api/fields");
        fieldsResp.EnsureSuccessStatusCode();
        var json = await fieldsResp.Content.ReadAsStringAsync();
        // Basic sanity check: expecting JSON array
        Assert.StartsWith("[", json.Trim());
    }
}
