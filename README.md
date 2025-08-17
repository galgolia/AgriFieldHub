# AgriFieldHub

ASP.NET Core 9 (Web API) backend for managing agricultural fields and their controllers.

## Features
- .NET 9 ASP.NET Core Web API
- Entity Framework Core (SQL Server) with Repository + Unit of Work
- Entities: User, Field, Controller (device)
- JWT Authentication (HS256) + Roles (Admin / Customer)
- PBKDF2 password hashing (salt:hash format)
- Ownership enforcement: users access only their own Fields & Controllers (Admins see all)
- Swagger (dev) with JWT Authorize support
- Basic integration tests (xUnit + WebApplicationFactory)

## Getting Started
### Prerequisites
- .NET 9 SDK
- SQL Server (local or container)

### Configuration
Edit `appsettings.json` (or override with environment variables / user secrets):
```
ConnectionStrings:DefaultConnection=Server=localhost;Database=AgriFieldHubDb;Trusted_Connection=True;TrustServerCertificate=True;
Jwt:Key=<32+ byte secret>
Jwt:Issuer=AgriFieldHub
Jwt:Audience=AgriFieldHubUsers
Jwt:ExpiryMinutes=60
```
IMPORTANT: Do not commit real secrets. Move `Jwt:Key` to environment variable before production.

### Database
Apply migrations:
```
dotnet ef database update --project AgriFieldHub/AgriFieldHub.csproj --startup-project AgriFieldHub/AgriFieldHub.csproj
```
The initial admin user is seeded:
```
Email: admin@example.com
Password: (seeded hash corresponds to internal test password; change immediately)
```
To change admin password run an UPDATE on Users or replace the seed and add a migration.

### Run
```
dotnet run --project AgriFieldHub/AgriFieldHub.csproj
```
Swagger UI (Development): https://localhost:<port>/swagger

### Auth Flow
1. Register: POST /api/auth/register { email, password }
2. Login: POST /api/auth/login -> returns JWT
3. Use JWT in Authorization: Bearer <token>

### Ownership Rules
- Non-admin users see only their own Fields and Controllers.
- Admins (Role=Admin) can see and manage all resources.

## Tests
Integration tests project: `AgriFieldHub.Tests`
Run tests:
```
dotnet test
```
Coverage (optional):
```
dotnet test /p:CollectCoverage=true
```

## Project Structure
- Controllers/: API endpoints
- Data/: EF Core DbContext
- Models/: Entity classes
- Repositories/: Generic + specific repositories & UnitOfWork
- DTOs/: Transport objects for requests/responses
- Services/: JWT token generation
- AgriFieldHub.Tests/: integration tests

## Improvements / TODO
- Central authorization policy (OwnerOrAdmin)
- Input validation (FluentValidation)
- Better password hashing / ASP.NET Identity
- More tests (controller CRUD, negative cases, admin access)
- Pagination & filtering
- CI workflow (GitHub Actions)
- Secret management (env vars / Key Vault)

## License
Add a LICENSE file if you intend to open source.
