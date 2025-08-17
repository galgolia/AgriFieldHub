using System.Security.Cryptography;
using System.Text;
using AgriFieldHub.DTOs;
using AgriFieldHub.Models;
using AgriFieldHub.Repositories;
using AgriFieldHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriFieldHub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenGenerator _tokenGen;

    public AuthController(IUnitOfWork uow, IJwtTokenGenerator tokenGen)
    {
        _uow = uow;
        _tokenGen = tokenGen;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var existing = await _uow.Users.GetByEmailAsync(request.Email, ct);
        if (existing != null) return Conflict("Email already registered");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = UserRole.Customer
        };
        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync();

        var token = _tokenGen.GenerateToken(user);
        return Ok(new AuthResponse { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(60) });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email, ct);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _tokenGen.GenerateToken(user);
        return Ok(new AuthResponse { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(60) });
    }

    private static string HashPassword(string password)
    {
        // Simple PBKDF2 (improve with salt storage in production)
        using var rng = RandomNumberGenerator.Create();
        Span<byte> salt = stackalloc byte[16];
        rng.GetBytes(salt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt.ToArray(), 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var test = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, test);
    }
}
