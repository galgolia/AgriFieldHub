# AgriFieldHub

ASP.NET Core 9 (net9.0) Web API project.

## Features
- Entity Framework Core (SQL Server) with Repository + Unit of Work pattern
- JWT Bearer Authentication wiring (no endpoints yet)
- OpenAPI (development only) via Minimal API mapping
- Initial EF Core migration with seeded admin user (placeholder hash)

## Prerequisites
- .NET 9 SDK
- SQL Server local instance (Default connection targets: `Server=localhost;Database=AgriFieldHubDb;Trusted_Connection=True;`)
- (Optional) dotnet-ef CLI: `dotnet tool install --global dotnet-ef`

## Configuration
Update `appsettings.json`:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AgriFieldHubDb;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "<REPLACE_WITH_SECURE_RANDOM>",
  "Issuer": "AgriFieldHub",
  "Audience": "AgriFieldHubUsers",
  "ExpiryMinutes": 60
}
```
Generate a secure JWT key (at least 32 chars). Do NOT commit secrets.

## Database
Apply migrations (after cloning):
```
dotnet ef database update --project AgriFieldHub/AgriFieldHub.csproj --startup-project AgriFieldHub/AgriFieldHub.csproj
```
Add new migration when model changes:
```
dotnet ef migrations add <Name> --project AgriFieldHub/AgriFieldHub.csproj --startup-project AgriFieldHub/AgriFieldHub.csproj
```

## Running
```
dotnet run --project AgriFieldHub/AgriFieldHub.csproj
```
OpenAPI (dev): `/openapi/v1.json`

## Next Steps
- Implement auth endpoints (register/login) with proper password hashing
- Add DTOs & validation pipeline
- Introduce logging & exception handling middleware
- Add GitHub Actions CI pipeline
