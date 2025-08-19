# AgriFieldHub – Design Overview

## 1. Goals
Provide a concise, secure, extensible backend API to manage Users, agricultural Fields, and device Controllers (e.g., irrigation units, sensors). Enforce row?level ownership: regular users access only their own resources; admins can access everything. Keep the codebase clear and interview?friendly.

## 2. Architecture Summary
Layered / modular organization inside a single ASP.NET Core Web API project.

| Layer | Responsibility |
|-------|----------------|
| Controllers | HTTP endpoints, minimal orchestration, authorization decisions. |
| DTOs | External API contracts decoupled from persistence entities. |
| Services | Cross?cutting domain services (JWT token generation). |
| Repositories + UoW | Data access abstraction (generic CRUD + specific queries + transactional boundary). |
| Data (DbContext) | EF Core model, relationships, migrations, seeding. |
| Models (Entities) | Persistence/domain entities (simple POCOs with annotations). |
| Tests | Integration tests validating auth + authorization behavior. |

## 3. Technology Choices
- **ASP.NET Core (.NET 9)**: Modern, minimal hosting model.
- **Entity Framework Core (SQL Server)**: Rapid data modeling, LINQ, migrations.
- **JWT (HS256)**: Stateless authentication suitable for SPA / mobile clients.
- **PBKDF2 hashing**: Simple self?contained password security (salt:hash); easily swappable.
- **xUnit + WebApplicationFactory**: Integration tests through real pipeline.
- **Swashbuckle/Swagger**: API discovery & manual testing.

## 4. Data Model
Entities & relationships:
- **User** (Id, Email, PasswordHash, Role[Admin|Customer], CreatedAt)
- **Field** (Id, Name, Description, Location, UserId FK, CreatedAt)
- **Controller** (Id, Type, Description, Status, FieldId FK, CreatedAt)

Relations: `User 1..* Field`, `Field 1..* Controller`. Cascade delete from user?fields and field?controllers to keep referential integrity.

## 5. Ownership & Authorization
- JWT includes claims: Sub, NameIdentifier (user id), Email, Role.
- Controllers enforce rules inline:
  - Non?admin requests are filtered by current user id.
  - Admin bypasses ownership checks.
- Simplicity was favored over adding a custom `IAuthorizationHandler`; this is a natural next enhancement.

## 6. Repository & Unit of Work Pattern
Reasons:
1. Encapsulation of EF queries & future persistence swap potential.
2. Central place to extend common querying patterns (tracking flags, cancellation tokens, existence checks).
3. UnitOfWork coordinates multiple repositories within a single request.

Trade?off: Slightly more boilerplate vs. directly using DbContext in controllers. The boilerplate is acceptable for clarity in a coding assignment.

## 7. Password & Security
- PBKDF2 with 100k iterations, 16?byte salt, 32?byte derived key (SHA256). Stored as `Base64Salt:Base64Hash`.
- Placeholder admin seed hash included for demo; real deployment must rotate and move password/JWT key to secure secret storage.
- Minimal clock skew (1 minute) for token validation.

## 8. Error Handling & Validation (Current State)
- Basic automatic model validation via data annotations.
- No global exception middleware (future: add ProblemDetails mapping + correlation id logging).

## 9. Testing Strategy
Implemented Integration tests:
- Registration/login path returns token.
- Ownership isolation (user cannot view another user's fields).
Future recommended tests:
- Negative login (wrong password, duplicate email).
- Admin listing all fields.
- Controller ownership & reassignment.
- Pagination once implemented.

## 10. Performance & Scalability Considerations
Current scale assumptions are small. Planned improvements if scaling up:
- Pagination (Skip/Take) for collection endpoints.
- Projection (select specific columns) or DTO mapping library (e.g., Mapster / AutoMapper).
- Caching hot reads (MemoryCache/Redis) for admin dashboards.
- Async database queries already in place with cancellation tokens for early abort when client disconnects.

## 11. Migrations & Seeding
- EF Core migrations track schema.
- One admin user seeded (can be removed for production; alternative: runtime bootstrap if environment variable present).
- Removal of intermediate migrations was done to simplify history for the assignment—would not do mid?project in production without a squashed baseline & coordinated deployment.

## 12. Logging & Observability
- Basic ASP.NET Core logging (Information / Warning). Next steps: structured logging (Serilog), OpenTelemetry tracing, health checks.

## 13. Security Hardening Roadmap
| Area | Next Step |
|------|-----------|
| Secrets | Move JWT key & admin password seed to environment / secret manager. |
| Passwords | Optionally integrate ASP.NET Identity / bcrypt / Argon2 with password upgrade path. |
| Authorization | Central policy (OwnerOrAdmin) + resource handlers. |
| Tokens | Add refresh tokens & key rotation strategy. |
| Input Validation | FluentValidation for richer errors & consistent problem details. |
| Rate Limiting | ASP.NET Core rate limiting middleware for brute force mitigation. |

## 14. Alternatives Considered
- **Skip repository layer**: Quicker but less explicit separation for tests – opted for clarity.
- **ASP.NET Identity**: Powerful but heavier; chosen to roll lightweight auth for assignment scope.
- **RS256 JWT**: More secure key management but adds certificate handling overhead; HS256 acceptable for scoped exercise.

## 15. Future Enhancements (Short List)
1. AuthorizationHandler / policy abstraction.
2. Global exception & ProblemDetails middleware.
3. Pagination & filtering endpoints.
4. Test suite expansion & code coverage gating in CI.
5. Environment?specific configuration & secrets via user secrets / Key Vault.

## 16. Build & Run Quick Reference
```
dotnet restore
(dotnet ef database update)  # ensure SQL Server running
dotnet run --project AgriFieldHub/AgriFieldHub.csproj
# Swagger (dev): /swagger
```

## 17. Submission Notes
- JWT key intentionally placeholder – replace via environment variable before production.
- Admin seed present for reviewer convenience; remove or rotate credentials post?evaluation.

---
This document provides reviewers with rationale behind structural and design decisions, making the codebase easier to navigate and extend.
