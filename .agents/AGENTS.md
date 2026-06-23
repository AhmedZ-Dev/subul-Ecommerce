# Subul-Ecommerce Workspace Rules

This file defines the rules and guidelines for building and testing the Subul-Ecommerce application. Antigravity automatically detects and enforces these rules.

## Core Architecture

**Stack:** ASP.NET Core 10 + PostgreSQL + EF Core + MediatR. Backend in `backend/`. Tests in `backend.Tests/`. Frontend planned — not built.

**Template:** `backend/Features/CategoryFeature/` + matching tests in `backend.Tests/Features/CategoryFeature/`.
**Schema:** `DATABASE.md` (one table section). **Seed example:** `productForExample.sql`.

### Structure

```
backend/
├── Common/              Result, ApiResponse, middleware, ValidationBehavior
├── DependencyInjection/ AddApplication(), AddInfrastructure()
├── Domain/Entities/     51 scaffolded partial classes
├── Features/{Entity}Feature/{Operation}/  *Endpoint.cs + Handler + Command|Query
└── Infrastructure/Persistence/  AppDbContext.cs (scaffold) + AppDbContext.Partial.cs (edit)

backend.Tests/
├── Infrastructure/      DatabaseFixture, DatabaseCollection, TestWebApplicationFactory
└── Features/{Entity}Feature/  Handler tests + {Entity}IntegrationTests.cs

.github/workflows/ci.yml   build + test + CodeQL + dependency audit + Gitleaks
```

### Flow

`Endpoint → ISender → ValidationBehavior → Handler(AppDbContext) → Result<T> → ToActionResult() → ApiResponse<T>`

Failure strings: `"not found"`→404 | `"already exists"`→409 | else→400. POST → `StatusCodes.Status201Created`.

`Program.cs` ends with `public partial class Program { }` — required for `WebApplicationFactory<Program>`.

### Naming Conventions

| Item | Pattern | Example |
|------|---------|---------|
| Feature | `{Entity}Feature` | `CategoryFeature` |
| Operation | `{Verb}{Entity}` | `CreateCategory` |
| Files | `{Op}Endpoint.cs`, `{Op}Handler.cs`, `{Op}Command.cs` | folder `CreateCategory/` |
| Class in Endpoint file | `{Op}Controller` | `CreateCategoryController` in `CreateCategoryEndpoint.cs` |
| Route | `api/{plural}` | `api/categories` |
| Namespace | `backend` | lowercase root |

### Scan Limits

| Task | Read only |
|------|-----------|
| New CRUD entity | `CategoryFeature/` (5 ops), `Domain/Entities/{Entity}.cs`, one `DATABASE.md` section |
| Edit one operation | That op's 3 files under `Features/{Entity}Feature/{Op}/` |
| Add tests | Mirror file in `backend.Tests/Features/CategoryFeature/` + `Infrastructure/DatabaseFixture.cs` |
| DB config | `AppDbContext.Partial.cs` only |

**Never read:** all 51 entities, full `AppDbContext.cs` (~2300 lines), unrelated features, `bin/`/`obj/`, frontend search (no `frontend/` yet).

---

## Backend Feature Implementation Rules

Copy `Features/CategoryFeature/` — 5 operations × 3 files each:

| Operation | Route | Folder |
|-----------|-------|--------|
| List{Entity}Paginated | GET `api/{entities}` | `ListCategoryPaginated/` |
| Create{Entity} | POST `api/{entities}` | `CreateCategory/` |
| GetById{Entity} | GET `api/{entities}/{id:long}` | `GetByIdCategory/` |
| Update{Entity} | PUT `api/{entities}/{id:long}` | `UpdateCategory/` |
| Delete{Entity} | DELETE `api/{entities}/{id:long}` | `DeleteCategory/` |

### Controller (`*Endpoint.cs` → class `*Controller`)

- `[ApiController]` + `[Route("api/…")]` + `[Tags("…")]`
- Primary ctor `(ISender sender)` — zero business logic
- Create/Get/List/Delete: send Command/Query directly from body or route
- Update: `[FromBody] Update{Entity}Request` → map to `Update{Entity}Command` with route `id`
- `sender.Send(cmd)` → `result.ToActionResult()` (201 for POST)

### Handler (`*Handler.cs`)

- `IRequestHandler<T, Result<TResponse>>` + `(AppDbContext context)`
- Reads: `AsNoTracking()`. Writes: tracked + `SaveChangesAsync(cancellationToken)`
- Trim: `command.NameEn.Trim()`. Case checks: `.ToLower()` (Category pattern)
- Bilingual search: both `NameEn` and `NameAr` (`ListCategoryPaginatedHandler.cs`)
- List defaults: `Page=1`, `Limit=10` → `(items, total, page, limit, totalPages)`
- Delete: FK/children guard first (`DeleteCategoryHandler.cs`)
- Slug: normalize + uniqueness loop (`CreateCategoryHandler.cs`)
- Timestamps: `DateTime.Now`

### Records

Command/Query + Response co-located in `{Op}Command.cs` or `{Op}Query.cs`.
Update adds separate `Update{Entity}Request` in same file as command.

### Validation

- **Category:** all domain rules in handler (uniqueness, FK, slug, delete guards)
- **Shape validation:** `ValidationBehavior` + FluentValidation registered — add `AbstractValidator<T>` per command when needed; throws → middleware 400 + `errors[]`
- Do not move domain rules (uniqueness, FK) into validators

---

## Frontend Rules

**Status:** `frontend/` does not exist. Do not search for React/Next.js in repo.

### When Adding Frontend
- Location: `frontend/` at repo root (Next.js + TypeScript per `.gitignore`)
- API: `process.env.NEXT_PUBLIC_API_URL` — dev default `http://localhost:5101`
- Mirror backend: `frontend/src/features/{entity}/` ↔ `Features/{Entity}Feature/`

### API Contract

```typescript
type ApiResponse<T> = { success: boolean; message: string | null; data: T | null; errors: string[] | null };
type Paginated<T> = { items: T[]; total: number; page: number; limit: number; totalPages: number };
```

- Gate on `success` before reading `data`
- Display: `nameAr` when locale=`ar`, else `nameEn` — enable RTL
- Server Components for fetch; `"use client"` only for interactivity/forms

### Scale
- One feature folder per entity — no cross-imports between features
- Shared UI → `frontend/src/components/ui/` only
- API client per entity → `frontend/src/features/{entity}/api.ts`
- Never duplicate backend rules (slugs, delete guards, uniqueness)

---

## Security

### Auth (not implemented)
All routes public. JWT package referenced; `Program.cs` has `UseAuthorization()` but no `UseAuthentication()`.
Before `[Authorize]`: wire JWT in `Program.cs`, bcrypt `PasswordHash` (`User`, `AdminUser`), log admin actions to `ActivityLog`.

### Secrets — never commit
`.env*`, `appsettings.*.local.json`, `secrets.json`, `*.pem`, `*.key`, `*.pfx`
Production DB creds via env/secret manager — not `appsettings.Development.json`.

### Input
- Input via Command/Query records (Update via Request → Command mapping)
- EF Core only for DB — no string-concat SQL
- `ToLower()` + trim for case-insensitive checks (Category pattern)
- Never return `PasswordHash` or tokens in responses

### Errors
`Result.Failure` for business errors — no stack traces. Middleware returns generic message on 500.
FluentValidation → 400 with `errors[]` via `ExceptionHandlingMiddleware`.

### CI security
`.github/workflows/ci.yml`: CodeQL (C#), `dotnet list package --vulnerable`, Gitleaks on push/PR.

### Future
- `pages.content_*`: sanitize HTML on write, escape on render (CMS)
- Payment: mask details; persist in `PaymentTransaction` only
- Role checks via `admin_users.role` when auth lands

---

## Testing

**Project:** `backend.Tests/` — xUnit + Testcontainers PostgreSQL + `Microsoft.AspNetCore.Mvc.Testing`.
**CI:** `.github/workflows/ci.yml` runs `dotnet test` on push/PR to `main`/`develop`. Requires Docker.

### Layout

```
backend.Tests/
├── Infrastructure/
│   DatabaseFixture.cs          # shared PostgreSQL container + EnsureCreated
│   DatabaseCollection.cs       # [CollectionDefinition("Database")]
│   TestWebApplicationFactory.cs
└── Features/{Entity}Feature/
    {Op}HandlerTests.cs
    {Entity}IntegrationTests.cs
```

### Handler Tests

```csharp
[Collection("Database")]
public class CreateCategoryHandlerTests(DatabaseFixture fixture)
{
    // fixture.CreateContext() → new Handler(context) → handler.Handle(cmd, CT)
}
```

Assert `result.IsSuccess` or error contains `"not found"` / `"already exists"`.
Cover: success, not found, already exists, delete-with-children (and entity-specific cases from Category).
Direct handler instantiation — do not mock MediatR.

### Integration Tests

`TestWebApplicationFactory(fixture.ConnectionString)` + `HttpClient`.
Assert HTTP status (201/404/409/400) + JSON `success`/`data` shape (`CategoryIntegrationTests.cs`).
Use `Guid.NewGuid()` in names to avoid collisions. `[Collection("Database")]` shares one container.

### Requirements
- Docker running locally for `dotnet test`
- `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` — in `DatabaseFixture` and `Program.cs`
- `public partial class Program { }` at end of `Program.cs`

### Skip
EF scaffold mapping tests, full JSON snapshots, `UseInMemoryDatabase`, mocking MediatR in HTTP tests.

---

## Anti-Patterns (recurring agent mistakes)

| Don't | Do instead |
|-------|------------|
| Assume JWT/`[Authorize]` works | Package installed; no `UseAuthentication()` in `Program.cs` |
| Assume FluentValidation on Category | Infra wired; Category validates inline in handlers — no validators yet |
| Guess entity fields | Read `Domain/Entities/{Entity}.cs` first (e.g. `Product` needs `Currency`, `Status` — no `IsActive`/`IsDigital`) |
| Rename `*Controller` ↔ `*Endpoint` | File: `{Op}Endpoint.cs`. Class: `{Op}Controller`. Keep both. |
| Use `UseInMemoryDatabase` for FK/delete tests | Testcontainers via `DatabaseFixture` |
| Use `DateTime.UtcNow` | `DateTime.Now` + `Npgsql.EnableLegacyTimestampBehavior` switch |
| Edit `AppDbContext.cs` | Scaffold only — use `AppDbContext.Partial.cs` |
| Create EF migrations | Schema via SQL + `DATABASE.md` |
| Add repository/service layer | Handlers inject `AppDbContext` directly |
| Throw for business errors | `Result.Failure("… not found")` |
| Use unused `Error` record | `Result.Failure(string)` |
| Register `LoggingBehavior` | File exists; not in DI |
| Query `Attribute` entity | DbContext alias: `AttributeEntity` |
| Extract shared DTOs early | Duplicate like Category until 3+ features need it |
| Build frontend before backend API | API first — no `frontend/` yet |
| Search entire repo for patterns | Grep within `Features/{Entity}Feature/` or `backend.Tests/Features/{Entity}Feature/` |
