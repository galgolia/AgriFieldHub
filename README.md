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
Edit `appsettings.json` or preferably override with environment variables / user secrets.
Example (PowerShell):
```
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=AgriFieldHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
$env:Jwt__Key="<your 32+ byte secret>"
$env:Jwt__Issuer="AgriFieldHub"
$env:Jwt__Audience="AgriFieldHubUsers"
```
The committed key in appsettings.json is a placeholder ONLY.

### Database
Apply migrations:
```
dotnet ef database update --project AgriFieldHub/AgriFieldHub.csproj --startup-project AgriFieldHub/AgriFieldHub.csproj
```
Seeded admin user (demo/testing):
```
Email: admin@example.com
Password: (hash placeholder, rotate after deployment)
```
To change admin password: UPDATE Users set PasswordHash=... or modify seed + add migration.

### Run
```
dotnet run --project AgriFieldHub/AgriFieldHub.csproj
```
Swagger UI (Development): https://localhost:<port>/swagger

### Auth Flow
1. Register: POST /api/auth/register { email, password }
2. Login: POST /api/auth/login -> JWT
3. Use JWT: Authorization: Bearer <token>

### Ownership Rules
- Non-admin: only own Fields & Controllers
- Admin: full visibility & management

## Sample cURL
Register:
```
curl -X POST https://localhost:7218/api/auth/register -H "Content-Type: application/json" -d '{"email":"u1@example.com","password":"Passw0rd!"}'
```
Login:
```
curl -X POST https://localhost:7218/api/auth/login -H "Content-Type: application/json" -d '{"email":"u1@example.com","password":"Passw0rd!"}'
```
List Fields:
```
curl -H "Authorization: Bearer <TOKEN>" https://localhost:7218/api/fields
```
Create Field:
```
curl -X POST https://localhost:7218/api/fields -H "Authorization: Bearer <TOKEN>" -H "Content-Type: application/json" -d '{"name":"My Field","description":"Demo"}'
```

## Tests
Run integration tests:
```
dotnet test
```
Add coverage:
```
dotnet test /p:CollectCoverage=true
```

## Project Structure
- Controllers/ (Auth, Users, Fields, Controllers)
- Data/ (DbContext)
- Models/ (Entities)
- Repositories/ (Generic + specialty + UoW)
- DTOs/ (Contracts)
- Services/ (JWT)
- Migrations/
- AgriFieldHub.Tests/ (Integration tests)

## Improvements / TODO
- Authorization policy handler (OwnerOrAdmin)
- Global exception / ProblemDetails middleware
- FluentValidation for DTOs
- Pagination & filtering
- CI workflow (GitHub Actions)
- Stronger password hashing provider (Identity / bcrypt / Argon2)
- Refresh tokens & key rotation

## License
Add a LICENSE file if open sourcing.
