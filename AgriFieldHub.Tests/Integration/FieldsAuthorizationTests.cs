using System.Net.Http.Headers;
using System.Net.Http.Json;
using AgriFieldHub.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AgriFieldHub.Tests.Integration;

public class FieldsAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public FieldsAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private async Task<string> RegisterAndLoginAsync(string email)
    {
        var client = _factory.CreateClient();
        var password = "Passw0rd!";
        await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest { Email = email, Password = password });
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = password });
        loginResp.EnsureSuccessStatusCode();
        var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }

    [Fact]
    public async Task User_Cannot_See_Others_Fields()
    {
        var tokenUser1 = await RegisterAndLoginAsync($"user1_{Guid.NewGuid():N}@example.com");
        var tokenUser2 = await RegisterAndLoginAsync($"user2_{Guid.NewGuid():N}@example.com");

        var client1 = _factory.CreateClient();
        client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUser1);
        // Create field for user1
        var createResp = await client1.PostAsJsonAsync("/api/fields", new CreateFieldRequest { Name = "Field A", Description = "Desc" });
        createResp.EnsureSuccessStatusCode();

        // User2 fetch fields -> should NOT contain Field A (list should be empty)
        var client2 = _factory.CreateClient();
        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUser2);
        var listResp = await client2.GetAsync("/api/fields");
        listResp.EnsureSuccessStatusCode();
        var listJson = await listResp.Content.ReadAsStringAsync();
        Assert.Equal("[]", listJson.Trim());
    }
}
