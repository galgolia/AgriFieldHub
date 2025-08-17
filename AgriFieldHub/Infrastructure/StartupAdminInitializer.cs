using System.Security.Cryptography;
using AgriFieldHub.Data;
using AgriFieldHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriFieldHub.Infrastructure;

public static class StartupAdminInitializer
{
    public static async Task EnsureAdminAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await ctx.Database.MigrateAsync(ct);

        if (await ctx.Users.AnyAsync(u => u.Role == UserRole.Admin, ct)) return;

        var password = Environment.GetEnvironmentVariable("ADMIN_SEED_PASSWORD");
        if (string.IsNullOrWhiteSpace(password)) return; // skip creating admin silently

        var (salt, hash) = HashPassword(password);
        var stored = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        ctx.Users.Add(new User
        {
            Email = "admin@example.com",
            PasswordHash = stored,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync(ct);
    }

    private static (byte[] salt, byte[] hash) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return (salt, hash);
    }
}
